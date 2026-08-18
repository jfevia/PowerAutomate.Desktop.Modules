// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Client request to open a private message channel with another player (opcode 0x9A).
/// </summary>
public sealed class ClientOpenPrivateChannelMessage : IClientMessage
{
    public ClientOpenPrivateChannelMessage(string receiverName)
    {
        if (receiverName == null)
        {
            throw new ArgumentNullException(nameof(receiverName));
        }

        ReceiverName = receiverName;
    }

    public string ReceiverName { get; }

    public byte Opcode => (byte)ClientOpcode.OpenPrivateChannel;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteString(ReceiverName);
    }
}
