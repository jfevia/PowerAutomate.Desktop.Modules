// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;
using PowerAutomate.Desktop.OpenTibia.Protocol;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client;

/// <summary>
/// Owns a live game connection: the socket, the reader thread, the writer and the inbound queue.
/// </summary>
public sealed class TibiaGameClient : IDisposable
{
    private const int HandshakeBufferSize = 4096;

    private readonly ISocketTransport _transport;
    private readonly GameServerMessageRegistry _registry;
    private readonly Func<uint[]> _keyFactory;
    private readonly MapFloorTracker _floors;
    private readonly ManualResetEventSlim _entered = new ManualResetEventSlim(false);

    private FrameWriter? _writer;
    private FrameReaderLoop? _reader;
    private string? _rejection;

    public TibiaGameClient(ISocketTransport transport)
        : this(transport, new MapFloorTracker())
    {
    }

    private TibiaGameClient(ISocketTransport transport, MapFloorTracker floors)
        : this(transport, GameServerRegistryFactory.CreateDefault(new DefaultItemTypeProvider(), floors), XteaKeyGenerator.Generate, floors)
    {
    }

    public TibiaGameClient(ISocketTransport transport, GameServerMessageRegistry registry, Func<uint[]> keyFactory)
        : this(transport, registry, keyFactory, new MapFloorTracker())
    {
    }

    /// <summary>
    /// Shares <paramref name="floors" /> with a caller-built registry, so the tracker stays current for its floor-aware readers.
    /// </summary>
    public TibiaGameClient(ISocketTransport transport, GameServerMessageRegistry registry, Func<uint[]> keyFactory, MapFloorTracker floors)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
        _floors = floors ?? throw new ArgumentNullException(nameof(floors));
        Filter = new OpcodeFilter();
    }

    public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    public OpcodeFilter Filter { get; }

    public ServerMessageQueue? Queue { get; private set; }

    /// <summary>
    /// Why the reader loop stopped, or null while the connection is healthy.
    /// </summary>
    public string? FaultReason => _reader?.FaultReason;

    /// <summary>
    /// The exception that stopped the reader, kept so a decode failure can be inspected.
    /// </summary>
    public Exception? Fault => _reader?.Fault;

    public void EnterGame(GameOptions options, TimeSpan timeout)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (State != ConnectionState.Disconnected)
        {
            throw new InvalidOperationException("The client is already connected.");
        }

        var key = _keyFactory();
        Queue = new ServerMessageQueue(options.QueueCapacity);

        State = ConnectionState.Connecting;
        _transport.Connect(options.Host, options.Port, timeout);

        State = ConnectionState.Authenticating;
        var challenge = ReadChallenge(timeout, out var leftover);

        var enter = new ClientGameEnterMessage(
            options.Os,
            options.Version,
            key,
            options.IsGamemaster,
            options.AccountName,
            options.CharacterName,
            options.Password,
            challenge.Timestamp,
            challenge.Random);

        var writer = new PacketWriter();
        enter.Write(writer);
        _transport.Write(FrameCodec.EncodePlain(writer.ToArray()));

        var pipeline = new InboundPipeline(_registry, Filter, Queue);
        pipeline.UseEncryption(key);

        _writer = new FrameWriter(_transport);
        _writer.UseEncryption(key);

        State = ConnectionState.EnteringGame;

        // The challenge read may have pulled in the first encrypted bytes; they must not be dropped.
        foreach (var message in pipeline.Push(leftover, 0, leftover.Length))
        {
            OnMessage(message);
        }

        _reader = new FrameReaderLoop(_transport, pipeline, Queue, OnMessage);
        _reader.Start();

        WaitUntilInGame(timeout);
    }

    public void Send(IClientMessage message)
    {
        if (State != ConnectionState.InGame)
        {
            throw new InvalidOperationException($"Cannot send while the connection is {State}.");
        }

        _writer!.Send(message);
    }

    /// <summary>
    /// Why the last logout attempt could not be written, or null when it succeeded or was skipped.
    /// </summary>
    public string? ExitFailure { get; private set; }

    public void ExitGame()
    {
        // Any recorded fault, exception or clean peer close, means writing a logout would be pointless.
        if (State == ConnectionState.InGame && FaultReason == null)
        {
            State = ConnectionState.Disconnecting;
            TrySendLogout();
        }

        Disconnect();
    }

    private void TrySendLogout()
    {
        try
        {
            _writer!.Send(ClientMessages.Logout());
        }
        catch (IOException exception)
        {
            ExitFailure = exception.Message;
        }
        catch (InvalidOperationException exception)
        {
            ExitFailure = exception.Message;
        }
    }

    /// <summary>
    /// Force-closes everything; safe to call repeatedly and after a fault.
    /// </summary>
    public void Disconnect()
    {
        _reader?.Stop();
        _transport.Close();
        Queue?.Complete();
        State = ConnectionState.Disconnected;
    }

    public void Dispose()
    {
        Disconnect();
        _reader?.Dispose();
        _entered.Dispose();
        _transport.Dispose();
    }

    private GameServerChallengeMessage ReadChallenge(TimeSpan timeout, out byte[] leftover)
    {
        var pending = new List<byte>();
        var buffer = new byte[HandshakeBufferSize];
        var clock = Stopwatch.StartNew();

        while (clock.Elapsed < timeout)
        {
            var read = _transport.Read(buffer, 0, buffer.Length);

            if (read < 0)
            {
                continue;
            }

            if (read == 0)
            {
                throw new ProtocolException("The game server closed the connection before sending a challenge.");
            }

            for (var index = 0; index < read; index++)
            {
                pending.Add(buffer[index]);
            }

            if (pending.Count < FrameCodec.LengthPrefixSize)
            {
                continue;
            }

            var declared = pending[0] | (pending[1] << 8);
            var total = FrameCodec.LengthPrefixSize + declared;
            if (pending.Count < total)
            {
                continue;
            }

            var body = new byte[declared];
            pending.CopyTo(FrameCodec.LengthPrefixSize, body, 0, declared);

            leftover = new byte[pending.Count - total];
            pending.CopyTo(total, leftover, 0, leftover.Length);

            var payload = FrameCodec.DecodeInboundPlain(body);
            var reader = new PacketReader(payload);
            var opcode = reader.ReadByte();

            if (opcode != (byte)GameServerOpcode.Challenge)
            {
                throw new ProtocolException($"Expected a challenge but received opcode 0x{opcode:X2}.");
            }

            return GameServerChallengeMessage.Read(GameServerOpcode.Challenge, reader);
        }

        throw new TimeoutException($"No challenge arrived within {timeout.TotalMilliseconds} ms.");
    }

    private void WaitUntilInGame(TimeSpan timeout)
    {
        var signalled = _entered.Wait(timeout);

        if (_rejection != null)
        {
            Disconnect();
            throw new GameLoginRejectedException(_rejection);
        }

        if (!signalled)
        {
            var fault = _reader!.Fault;
            Disconnect();

            if (fault != null)
            {
                throw new ProtocolException(
                    "The reader stopped before entry was confirmed: " + fault.Message, fault);
            }

            throw new TimeoutException($"The game server did not confirm entry within {timeout.TotalMilliseconds} ms.");
        }

        State = ConnectionState.InGame;
    }

    private void OnMessage(IProtocolMessage message)
    {
        // Runs first so the tracker is current before the next map message is decoded, handshake leftovers included.
        _floors.Observe(message);

        if (message.Opcode == (byte)GameServerOpcode.Ping)
        {
            _writer!.Send(ClientMessages.PingBack());
            return;
        }

        if (message is GameServerLoginErrorMessage rejected)
        {
            _rejection = rejected.ErrorText;
            _entered.Set();
            return;
        }

        if (message is GameServerLoginOrPendingStateMessage)
        {
            _entered.Set();
        }
    }
}
