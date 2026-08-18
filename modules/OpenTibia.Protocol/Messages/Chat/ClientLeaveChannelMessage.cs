// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Client request to leave a public channel (opcode 0x99).
/// </summary>
public sealed class ClientLeaveChannelMessage : IClientMessage
{
    public ClientLeaveChannelMessage(ushort channelId)
    {
        ChannelId = channelId;
    }

    public ushort ChannelId { get; }

    public byte Opcode => (byte)ClientOpcode.LeaveChannel;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteUInt16(ChannelId);
    }
}
