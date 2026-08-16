// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

/// <summary>
/// Coalesces several client messages into one frame payload, the way the real client batches input.
/// </summary>
public static class OutboundFrameWriter
{
    public static byte[] Compose(IReadOnlyList<IClientMessage> messages)
    {
        if (messages == null)
        {
            throw new ArgumentNullException(nameof(messages));
        }

        if (messages.Count == 0)
        {
            throw new ArgumentException("At least one message must be supplied.", nameof(messages));
        }

        var writer = new PacketWriter();

        for (var index = 0; index < messages.Count; index++)
        {
            var message = messages[index];
            if (message == null)
            {
                throw new ArgumentException($"Message at index {index} is null.", nameof(messages));
            }

            message.Write(writer);
        }

        return writer.ToArray();
    }
}
