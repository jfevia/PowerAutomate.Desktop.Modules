// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

namespace PowerAutomate.Desktop.OpenTibia.Client.Transport;

/// <summary>
/// Serializes outbound messages behind a lock, because flow actions and the auto-ping can race.
/// </summary>
public sealed class FrameWriter
{
    private readonly object _gate = new object();
    private readonly ISocketTransport _transport;

    private uint[]? _key;

    public FrameWriter(ISocketTransport transport)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
    }

    public bool IsEncrypted => _key != null;

    public void UseEncryption(uint[] key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        lock (_gate)
        {
            _key = key;
        }
    }

    public void Send(IClientMessage message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var writer = new PacketWriter();
        message.Write(writer);
        SendPayload(writer.ToArray());
    }

    public void SendPayload(byte[] payload)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        lock (_gate)
        {
            var frame = _key == null
                ? FrameCodec.EncodePlain(payload)
                : FrameCodec.EncodeEncrypted(payload, _key);

            _transport.Write(frame);
        }
    }
}
