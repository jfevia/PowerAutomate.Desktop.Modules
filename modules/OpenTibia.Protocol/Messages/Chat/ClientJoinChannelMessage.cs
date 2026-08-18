// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Client request to join a public channel (opcode 0x98).
/// </summary>
public sealed class ClientJoinChannelMessage : IClientMessage
{
    public ClientJoinChannelMessage(ushort channelId)
    {
        ChannelId = channelId;
    }

    public ushort ChannelId { get; }

    public byte Opcode => (byte)ClientOpcode.JoinChannel;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteUInt16(ChannelId);
    }
}
