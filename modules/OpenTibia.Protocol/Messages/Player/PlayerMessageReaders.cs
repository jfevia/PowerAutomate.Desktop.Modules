// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Player;

/// <summary>
/// Registers the player-state message readers (stats, skills, status icons).
/// </summary>
public static class PlayerMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(GameServerOpcode.PlayerData, GameServerPlayerDataMessage.Read);
        registry.Register(GameServerOpcode.PlayerSkills, GameServerPlayerSkillsMessage.Read);
        registry.Register(GameServerOpcode.PlayerState, GameServerPlayerStateMessage.Read);
    }
}
