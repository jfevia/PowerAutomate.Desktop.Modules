// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Social;

/// <summary>
/// Registers the VIP contact notice message readers.
/// </summary>
public static class SocialMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(GameServerOpcode.VipAdd, GameServerVipAddMessage.Read);
        registry.Register(GameServerOpcode.VipState, GameServerVipStateMessage.Read);
        registry.Register(GameServerOpcode.VipLogout, GameServerVipLogoutMessage.Read);
    }
}
