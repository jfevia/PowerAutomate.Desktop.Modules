// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Server notice that a channel was closed for the local player (s2c 0xB3).
/// </summary>
public sealed class GameServerCloseChannelMessage : IProtocolMessage
{
    public GameServerCloseChannelMessage(ushort channelId)
    {
        ChannelId = channelId;
    }

    public byte Opcode => (byte)GameServerOpcode.CloseChannel;

    public ushort ChannelId { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var channelId = reader.ReadUInt16();
        return new GameServerCloseChannelMessage(channelId);
    }
}
