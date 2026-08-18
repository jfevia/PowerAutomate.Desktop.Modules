// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Registers the chat and channel notice message readers.
/// </summary>
public static class ChatMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(GameServerOpcode.TextMessage, GameServerTextMessage.Read);
        registry.Register(GameServerOpcode.Talk, GameServerTalkMessage.Read);
        registry.Register(GameServerOpcode.Channels, GameServerChannelsMessage.Read);
        registry.Register(GameServerOpcode.OpenChannel, GameServerOpenChannelMessage.Read);
        registry.Register(GameServerOpcode.CloseChannel, GameServerCloseChannelMessage.Read);
        registry.Register(GameServerOpcode.OpenPrivateChannel, GameServerOpenPrivateChannelMessage.Read);
        registry.Register(GameServerOpcode.OpenOwnChannel, GameServerOpenOwnChannelMessage.Read);
    }
}
