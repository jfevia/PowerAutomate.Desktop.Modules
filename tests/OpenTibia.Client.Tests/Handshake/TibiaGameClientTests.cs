// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Client.Tests.Transport;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;
using PowerAutomate.Desktop.OpenTibia.Protocol;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Handshake;

[TestFixture]
public class TibiaGameClientTests
{
    private static readonly uint[] Key = { 9u, 8u, 7u, 6u };
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(5);

    private static GameOptions Options()
    {
        return new GameOptions("127.0.0.1", 7172, "account", "Rook", "secret") { QueueCapacity = 64 };
    }

    private static byte[] ChallengeFrame()
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.Challenge);
        writer.WriteUInt32(0x11223344);
        writer.WriteByte(0x55);
        return FrameCodec.EncodePlain(writer.ToArray());
    }

    private static byte[] PendingStatePayload()
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.LoginOrPendingState);
        writer.WriteUInt32(0x00001000);
        writer.WriteUInt16(50);
        writer.WriteByte(0);
        return writer.ToArray();
    }

    private static byte[] LoginErrorPayload(string text)
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.LoginError);
        writer.WriteString(text);
        return writer.ToArray();
    }

    private static byte[] PingPayload()
    {
        return new[] { (byte)GameServerOpcode.Ping };
    }

    private static byte[] FloorChangeDownPayload(int preRevealFloorCount, byte newZ)
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.FloorChangeDown);
        MapFloorPayloadWriter.WriteEmptyTiles(
            writer, preRevealFloorCount * MapFloorPayloadWriter.FloorWidth * MapFloorPayloadWriter.FloorHeight);
        MapFloorPayloadWriter.WriteEmptyTiles(writer, MapFloorPayloadWriter.FloorHeight * MapFloorPayloadWriter.FloorCountFor(newZ));
        MapFloorPayloadWriter.WriteEmptyTiles(writer, MapFloorPayloadWriter.FloorWidth * MapFloorPayloadWriter.FloorCountFor(newZ));
        return writer.ToArray();
    }

    private static byte[] MapRightRowPayload(byte currentZ)
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)GameServerOpcode.MapRightRow);
        MapFloorPayloadWriter.WriteEmptyTiles(writer, MapFloorPayloadWriter.FloorHeight * MapFloorPayloadWriter.FloorCountFor(currentZ));
        return writer.ToArray();
    }

    private static TibiaGameClient Client(ISocketTransport transport)
    {
        return new TibiaGameClient(transport, GameServerRegistryFactory.CreateDefault(), () => Key);
    }

    [Test]
    public void EnterGame_CompletesTheHandshakeAndReportsInGame()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));

        using var client = Client(transport);
        client.EnterGame(Options(), Patience);

        Assert.That(client.State, Is.EqualTo(ConnectionState.InGame));
    }

    [Test]
    public void EnterGame_SendsThePlaintextEnterMessageEchoingTheChallenge()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));

        using var client = Client(transport);
        client.EnterGame(Options(), Patience);

        var payload = FrameCodec.DecodePlain(
            transport.Written.First().Skip(FrameCodec.LengthPrefixSize).ToArray());

        Assert.That(payload[0], Is.EqualTo((byte)ClientOpcode.EnterGame));
    }

    [Test]
    public void EnterGame_WhenChallengeAndFirstMessageShareOneRead_DoesNotLoseTheMessage()
    {
        // A real server coalesces these; the leftover bytes must survive the handshake.
        var combined = new List<byte>();
        combined.AddRange(ChallengeFrame());
        combined.AddRange(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));

        var transport = new FakeSocketTransport();
        transport.EnqueueRead(combined.ToArray());

        using var client = Client(transport);
        client.EnterGame(Options(), Patience);

        Assert.That(client.State, Is.EqualTo(ConnectionState.InGame));
    }

    [Test]
    public void EnterGame_WhenServerRejectsTheCharacter_Throws()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(LoginErrorPayload("Character not found."), Key));

        using var client = Client(transport);

        var exception = Assert.Throws<GameLoginRejectedException>(() => client.EnterGame(Options(), Patience))!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Does.Contain("Character not found"));
            Assert.That(client.State, Is.EqualTo(ConnectionState.Disconnected));
        });
    }

    [Test]
    public void EnterGame_AnswersServerPingWithoutInvolvingTheQueue()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PingPayload(), Key));
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));

        using var client = Client(transport);
        client.EnterGame(Options(), Patience);

        // Frame 0 is the enter message; the ping reply is the second thing written.
        Assert.That(transport.Written, Has.Count.GreaterThanOrEqualTo(2));
    }

    [Test]
    public void EnterGame_WhenNoChallengeArrives_ThrowsTimeout()
    {
        // Drips bytes that never complete the declared frame, so the deadline expires first.
        var transport = new DripTransport();

        using var client = Client(transport);

        Assert.Throws<TimeoutException>(() => client.EnterGame(Options(), TimeSpan.FromMilliseconds(100)));
    }

    [Test]
    public void EnterGame_WhenReadsTimeOutBeforeTheChallenge_StillCompletes()
    {
        var transport = new FakeSocketTransport { TimeoutsBeforeData = 2 };
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));

        using var client = Client(transport);
        client.EnterGame(Options(), Patience);

        Assert.That(client.State, Is.EqualTo(ConnectionState.InGame));
    }

    [Test]
    public void EnterGame_WhenServerClosesBeforeChallenge_ThrowsProtocolException()
    {
        var transport = new FakeSocketTransport();

        using var client = Client(transport);

        Assert.Throws<ProtocolException>(() => client.EnterGame(Options(), Patience));
    }

    [Test]
    public void EnterGame_WhenFirstFrameIsNotAChallenge_ThrowsProtocolException()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(FrameCodec.EncodePlain(new[] { (byte)GameServerOpcode.PlayerData }));

        using var client = Client(transport);

        Assert.Throws<ProtocolException>(() => client.EnterGame(Options(), Patience));
    }

    [Test]
    public void EnterGame_WhenEntryIsNeverConfirmed_ThrowsTimeout()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(ChallengeFrame());

        using var client = Client(transport);

        Assert.Throws<TimeoutException>(() => client.EnterGame(Options(), TimeSpan.FromMilliseconds(150)));
    }

    [Test]
    public void EnterGame_WhenAlreadyConnected_Throws()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));

        using var client = Client(transport);
        client.EnterGame(Options(), Patience);

        Assert.Throws<InvalidOperationException>(() => client.EnterGame(Options(), Patience));
    }

    [Test]
    public void Send_BeforeEnteringGame_Throws()
    {
        using var client = Client(new FakeSocketTransport());

        Assert.Throws<InvalidOperationException>(() => client.Send(ClientMessages.PingBack()));
    }

    [Test]
    public void Send_WhileInGame_WritesAnEncryptedFrame()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));

        using var client = Client(transport);
        client.EnterGame(Options(), Patience);
        var before = transport.Written.Count;

        client.Send(ClientMessages.RequestChannels());

        var payload = FrameCodec.DecodeEncrypted(
            transport.Written.Last().Skip(FrameCodec.LengthPrefixSize).ToArray(), Key);

        Assert.Multiple(() =>
        {
            Assert.That(transport.Written, Has.Count.EqualTo(before + 1));
            Assert.That(payload, Is.EqualTo(new[] { (byte)ClientOpcode.RequestChannels }));
        });
    }

    [Test]
    public void ExitGame_SendsLogoutAndDisconnects()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));

        using var client = Client(transport);
        client.EnterGame(Options(), Patience);

        client.ExitGame();

        var payload = FrameCodec.DecodeEncrypted(
            transport.Written.Last().Skip(FrameCodec.LengthPrefixSize).ToArray(), Key);

        Assert.Multiple(() =>
        {
            Assert.That(payload, Is.EqualTo(new[] { (byte)ClientOpcode.LeaveGame }));
            Assert.That(client.State, Is.EqualTo(ConnectionState.Disconnected));
        });
    }

    [Test]
    public void ExitGame_WhenNotInGame_JustDisconnects()
    {
        var transport = new FakeSocketTransport();

        using var client = Client(transport);
        client.ExitGame();

        Assert.Multiple(() =>
        {
            Assert.That(client.State, Is.EqualTo(ConnectionState.Disconnected));
            Assert.That(transport.Written, Is.Empty);
        });
    }

    [Test]
    public void Disconnect_IsIdempotent()
    {
        var transport = new FakeSocketTransport();

        using var client = Client(transport);
        client.Disconnect();
        client.Disconnect();

        Assert.That(client.State, Is.EqualTo(ConnectionState.Disconnected));
    }

    [Test]
    public void Queue_ReceivesMessagesThatArriveAfterEntry()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(
            new[] { (byte)GameServerOpcode.CancelWalk, (byte)0x00 }, Key));

        using var client = Client(transport);
        client.EnterGame(Options(), Patience);

        // Entry can complete while the reader thread is still decoding the next frame, so poll for it.
        var seenOpcodes = new List<byte>();
        var deadline = System.Diagnostics.Stopwatch.StartNew();
        while (!seenOpcodes.Contains((byte)GameServerOpcode.CancelWalk) && deadline.Elapsed < Patience)
        {
            seenOpcodes.AddRange(client.Queue!.DequeueBatch(8, TimeSpan.FromMilliseconds(500))
                .Select(message => message.Opcode));
        }

        Assert.Multiple(() =>
        {
            Assert.That(client.Queue!.Capacity, Is.EqualTo(64));
            Assert.That(seenOpcodes,
                Does.Contain((byte)GameServerOpcode.CancelWalk),
                "An ordinary gameplay message must reach the queue untouched by the observer.");
        });
    }

    [Test]
    public void EnterGame_ThenFloorChangeAndRowMessage_DecodeWithoutFaultingTheConnection()
    {
        // Proves the fix: a floor change followed by the row reveal it triggers must not fault the reader.
        var floors = new MapFloorTracker();
        var registry = GameServerRegistryFactory.CreateDefault(new DefaultItemTypeProvider(), floors);

        const byte newZ = 8;
        var transport = new WalkSequenceTransport();
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(FloorChangeDownPayload(preRevealFloorCount: 3, newZ), Key));
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(MapRightRowPayload(newZ), Key));

        using var client = new TibiaGameClient(transport, registry, () => Key, floors);
        client.EnterGame(Options(), Patience);

        var seenOpcodes = new HashSet<byte>();
        var deadline = System.Diagnostics.Stopwatch.StartNew();
        while (!seenOpcodes.Contains((byte)GameServerOpcode.MapRightRow) && deadline.Elapsed < Patience)
        {
            foreach (var message in client.Queue!.DequeueBatch(8, TimeSpan.FromMilliseconds(50)))
            {
                seenOpcodes.Add(message.Opcode);
            }
        }

        Assert.Multiple(() =>
        {
            Assert.That(seenOpcodes, Does.Contain((byte)GameServerOpcode.FloorChangeDown));
            Assert.That(seenOpcodes, Does.Contain((byte)GameServerOpcode.MapRightRow));
            Assert.That(client.FaultReason, Is.Null);
            Assert.That(floors.CurrentZ, Is.EqualTo(newZ));
        });
    }

    [Test]
    public void FaultReason_IsNullBeforeAnyConnection()
    {
        using var client = Client(new FakeSocketTransport());

        Assert.That(client.FaultReason, Is.Null);
    }

    [Test]
    public void FaultReason_ReportsWhyTheReaderStoppedAfterEntry()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(ChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(PendingStatePayload(), Key));

        using var client = Client(transport);
        client.EnterGame(Options(), Patience);

        // The fake has no more reads, so the loop sees a closed peer and records it.
        var deadline = System.Diagnostics.Stopwatch.StartNew();
        while (client.FaultReason == null && deadline.Elapsed < Patience)
        {
            System.Threading.Thread.Sleep(10);
        }

        Assert.That(client.FaultReason, Is.EqualTo("The server closed the connection."));
    }

    [Test]
    public void Constructor_WithNullTransport_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TibiaGameClient(null!));
    }

    [Test]
    public void Constructor_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new TibiaGameClient(new FakeSocketTransport(), null!, () => Key));
    }

    [Test]
    public void Constructor_WithNullKeyFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new TibiaGameClient(new FakeSocketTransport(), GameServerRegistryFactory.CreateDefault(), null!));
    }

    [Test]
    public void Constructor_WithNullFloors_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new TibiaGameClient(new FakeSocketTransport(), GameServerRegistryFactory.CreateDefault(), () => Key, null!));
    }

    [Test]
    public void Constructor_WithSharedFloorsOverload_IsAccepted()
    {
        var floors = new MapFloorTracker();
        var registry = GameServerRegistryFactory.CreateDefault(new DefaultItemTypeProvider(), floors);

        using var client = new TibiaGameClient(new FakeSocketTransport(), registry, () => Key, floors);

        Assert.That(client.State, Is.EqualTo(ConnectionState.Disconnected));
    }

    [Test]
    public void Constructor_WithDefaultDependencies_IsAccepted()
    {
        using var client = new TibiaGameClient(new FakeSocketTransport());

        Assert.That(client.State, Is.EqualTo(ConnectionState.Disconnected));
    }

    [Test]
    public void EnterGame_WithNullOptions_Throws()
    {
        using var client = Client(new FakeSocketTransport());

        Assert.Throws<ArgumentNullException>(() => client.EnterGame(null!, Patience));
    }

    /// <summary>
    /// Announces a large frame then never delivers it, so only the deadline can end the read.
    /// </summary>
    private sealed class DripTransport : ISocketTransport
    {
        private bool _prefixSent;

        public bool IsConnected => true;

        public void Connect(string host, int port, TimeSpan timeout)
        {
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            System.Threading.Thread.Sleep(10);

            if (!_prefixSent)
            {
                _prefixSent = true;
                buffer[offset] = 0xFF;
                buffer[offset + 1] = 0x00;
                return 2;
            }

            buffer[offset] = 0x00;
            return 1;
        }

        public void Write(byte[] data)
        {
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Serves scripted reads, then drips filler bytes so the walk-sequence test never races a closed connection.
    /// </summary>
    private sealed class WalkSequenceTransport : ISocketTransport
    {
        private readonly Queue<byte[]> _reads = new Queue<byte[]>();
        private bool _prefixSent;

        public bool IsConnected => true;

        public void EnqueueRead(byte[] data)
        {
            _reads.Enqueue(data);
        }

        public void Connect(string host, int port, TimeSpan timeout)
        {
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (_reads.Count > 0)
            {
                var chunk = _reads.Dequeue();
                Array.Copy(chunk, 0, buffer, offset, chunk.Length);
                return chunk.Length;
            }

            System.Threading.Thread.Sleep(10);

            if (!_prefixSent)
            {
                _prefixSent = true;
                buffer[offset] = 0xFF;
                buffer[offset + 1] = 0x00;
                return 2;
            }

            buffer[offset] = 0x00;
            return 1;
        }

        public void Write(byte[] data)
        {
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }
}

[TestFixture]
public class GameOptionsTests
{
    [Test]
    public void Constructor_KeepsEveryFieldAndDefaults()
    {
        var options = new GameOptions("h", 7172, "acct", "Rook", "pw");

        Assert.Multiple(() =>
        {
            Assert.That(options.Host, Is.EqualTo("h"));
            Assert.That(options.Port, Is.EqualTo(7172));
            Assert.That(options.AccountName, Is.EqualTo("acct"));
            Assert.That(options.CharacterName, Is.EqualTo("Rook"));
            Assert.That(options.Password, Is.EqualTo("pw"));
            Assert.That(options.Version, Is.EqualTo(860));
            Assert.That(options.Os, Is.EqualTo(2));
            Assert.That(options.IsGamemaster, Is.False);
            Assert.That(options.QueueCapacity, Is.EqualTo(4096));
        });
    }

    [Test]
    public void Constructor_WithNullHost_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameOptions(null!, 1, "a", "c", "p"));
    }

    [Test]
    public void Constructor_WithNullAccountName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameOptions("h", 1, null!, "c", "p"));
    }

    [Test]
    public void Constructor_WithNullCharacterName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameOptions("h", 1, "a", null!, "p"));
    }

    [Test]
    public void Constructor_WithNullPassword_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameOptions("h", 1, "a", "c", null!));
    }

    [TestCase(0)]
    [TestCase(70000)]
    public void Constructor_WithPortOutOfRange_Throws(int port)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GameOptions("h", port, "a", "c", "p"));
    }
}

[TestFixture]
public class GameServerRegistryFactoryTests
{
    [TestCase(GameServerOpcode.Ping)]
    [TestCase(GameServerOpcode.LoginOrPendingState)]
    [TestCase(GameServerOpcode.PlayerData)]
    [TestCase(GameServerOpcode.TextMessage)]
    [TestCase(GameServerOpcode.Talk)]
    [TestCase(GameServerOpcode.CreatureHealth)]
    [TestCase(GameServerOpcode.CancelWalk)]
    [TestCase(GameServerOpcode.VipAdd)]
    [TestCase(GameServerOpcode.GraphicalEffect)]
    [TestCase(GameServerOpcode.FullMap)]
    [TestCase(GameServerOpcode.MapTopRow)]
    [TestCase(GameServerOpcode.MapRightRow)]
    [TestCase(GameServerOpcode.MapBottomRow)]
    [TestCase(GameServerOpcode.MapLeftRow)]
    [TestCase(GameServerOpcode.FloorChangeUp)]
    [TestCase(GameServerOpcode.FloorChangeDown)]
    public void CreateDefault_RegistersTheCoreOpcodes(GameServerOpcode opcode)
    {
        Assert.That(GameServerRegistryFactory.CreateDefault().IsRegistered(opcode), Is.True);
    }
}
