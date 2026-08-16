// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Server notice that the local player's own private channel was created (s2c 0xB2).
/// </summary>
public sealed class GameServerOpenOwnChannelMessage : IProtocolMessage
{
    public GameServerOpenOwnChannelMessage(ushort channelId, string channelName)
    {
        ChannelId = channelId;
        ChannelName = channelName ?? throw new ArgumentNullException(nameof(channelName));
    }

    public byte Opcode => (byte)GameServerOpcode.OpenOwnChannel;

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
        return new GameServerOpenOwnChannelMessage(channelId, channelName);
    }
}
