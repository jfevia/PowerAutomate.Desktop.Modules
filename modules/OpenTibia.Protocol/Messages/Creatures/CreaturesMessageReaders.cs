// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Registers the creature-state notice message readers (health, light, outfit, speed, markers).
/// </summary>
public static class CreaturesMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(GameServerOpcode.CreatureHealth, GameServerCreatureHealthMessage.Read);
        registry.Register(GameServerOpcode.CreatureLight, GameServerCreatureLightMessage.Read);
        registry.Register(GameServerOpcode.CreatureOutfit, GameServerCreatureOutfitMessage.Read);
        registry.Register(GameServerOpcode.CreatureSpeed, GameServerCreatureSpeedMessage.Read);
        registry.Register(GameServerOpcode.CreatureSkull, GameServerCreatureSkullMessage.Read);
        registry.Register(GameServerOpcode.CreatureParty, GameServerCreatureShieldMessage.Read);
        registry.Register(GameServerOpcode.CreatureSquare, GameServerCreatureSquareMessage.Read);
        registry.Register(GameServerOpcode.CreatureUnpass, GameServerCreatureUnpassMessage.Read);
    }
}
