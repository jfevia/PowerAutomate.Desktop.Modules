// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

/// <summary>
/// Shared guard used by every client message when it starts writing itself.
/// </summary>
public static class MessageWriter
{
    public static void WriteOpcode(PacketWriter writer, byte opcode)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        writer.WriteByte(opcode);
    }
}
