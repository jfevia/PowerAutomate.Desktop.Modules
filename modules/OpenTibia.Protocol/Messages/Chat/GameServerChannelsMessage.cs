// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Server list of every available public channel (s2c 0xAB).
/// </summary>
public sealed class GameServerChannelsMessage : IProtocolMessage
{
    public GameServerChannelsMessage(IReadOnlyList<ChannelListEntry> channels)
    {
        Channels = channels ?? throw new ArgumentNullException(nameof(channels));
    }

    public byte Opcode => (byte)GameServerOpcode.Channels;

    public IReadOnlyList<ChannelListEntry> Channels { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var channelCount = reader.ReadByte();
        var channels = new List<ChannelListEntry>(channelCount);
        for (var index = 0; index < channelCount; index++)
        {
            channels.Add(ChannelListEntry.Read(reader));
        }

        return new GameServerChannelsMessage(channels);
    }
}
