// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Server notice that a public channel was opened for the local player (s2c 0xAC).
/// </summary>
public sealed class GameServerOpenChannelMessage : IProtocolMessage
{
    public GameServerOpenChannelMessage(ushort channelId, string channelName)
    {
        ChannelId = channelId;
        ChannelName = channelName ?? throw new ArgumentNullException(nameof(channelName));
    }

    public byte Opcode => (byte)GameServerOpcode.OpenChannel;

    public ushort ChannelId { get; }

    public string ChannelName { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var channelId = reader.ReadUInt16();
        var channelName = reader.ReadString();
        return new GameServerOpenChannelMessage(channelId, channelName);
    }
}
