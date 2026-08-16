// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Protocol;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

namespace PowerAutomate.Desktop.OpenTibia.Client.Transport;

/// <summary>
/// Turns raw socket bytes into decoded messages and feeds the queue, without owning a thread.
/// </summary>
public sealed class InboundPipeline
{
    private readonly FrameBuffer _frames = new FrameBuffer();
    private readonly GameServerMessageRegistry _registry;
    private readonly OpcodeFilter _filter;
    private readonly ServerMessageQueue _queue;

    private uint[]? _key;

    public InboundPipeline(GameServerMessageRegistry registry, OpcodeFilter filter, ServerMessageQueue queue)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
    }

    /// <summary>
    /// True once the stream has switched from the plaintext login frame to XTEA.
    /// </summary>
    public bool IsEncrypted => _key != null;

    public void UseEncryption(uint[] key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        _key = key;
    }

    /// <summary>
    /// Consumes a socket read and returns every message it completed, filtered ones included.
    /// </summary>
    public IReadOnlyList<IProtocolMessage> Push(byte[] data, int offset, int count)
    {
        _frames.Append(data, offset, count);

        var decoded = new List<IProtocolMessage>();

        while (_frames.TryReadFrame(out var body))
        {
            var payload = _key == null
                ? FrameCodec.DecodeInboundPlain(body)
                : FrameCodec.DecodeEncrypted(body, _key);

            foreach (var message in ReadPayload(payload))
            {
                decoded.Add(message);

                if (_filter.IsAllowed(message.Opcode))
                {
                    _queue.Enqueue(message);
                }
                else
                {
                    _queue.RecordFiltered();
                }
            }
        }

        return decoded;
    }

    /// <summary>
    /// Attaches the offending payload to a decode failure, which is the only way to diagnose a live mismatch.
    /// </summary>
    private IReadOnlyList<IProtocolMessage> ReadPayload(byte[] payload)
    {
        try
        {
            return _registry.ReadAll(payload);
        }
        catch (ProtocolException exception)
        {
            throw new PayloadDecodeException(exception.Message, payload, exception);
        }
    }
}

/// <summary>
/// A decode failure that carries the whole frame payload so the bad bytes can be inspected.
/// </summary>
public class PayloadDecodeException : ProtocolException
{
    public PayloadDecodeException(string message, byte[] payload, Exception innerException)
        : base(message, innerException)
    {
        Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public byte[] Payload { get; }

    /// <summary>
    /// The payload as spaced hex, ready to paste into a bug report.
    /// </summary>
    public string PayloadHex
    {
        get
        {
            var hex = new StringBuilder(Payload.Length * 3);
            foreach (var value in Payload)
            {
                hex.Append(value.ToString("X2")).Append(' ');
            }

            return hex.ToString().TrimEnd();
        }
    }
}
