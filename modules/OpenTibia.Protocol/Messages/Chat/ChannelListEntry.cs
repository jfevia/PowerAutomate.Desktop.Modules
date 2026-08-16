// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// One entry within a <see cref="GameServerChannelsMessage"/> channel list.
/// </summary>
public sealed class ChannelListEntry
{
    public ChannelListEntry(ushort channelId, string channelName)
    {
        ChannelId = channelId;
        ChannelName = channelName ?? throw new ArgumentNullException(nameof(channelName));
    }

    public ushort ChannelId { get; }

    public string ChannelName { get; }

    public static ChannelListEntry Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var channelId = reader.ReadUInt16();
        var channelName = reader.ReadString();
        return new ChannelListEntry(channelId, channelName);
    }
}
