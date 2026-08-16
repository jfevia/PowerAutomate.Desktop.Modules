// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// Registers the visual-effect and miscellaneous notice message readers.
/// </summary>
public static class EffectsMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(GameServerOpcode.Ambient, GameServerAmbientMessage.Read);
        registry.Register(GameServerOpcode.GraphicalEffect, GameServerGraphicalEffectMessage.Read);
        registry.Register(GameServerOpcode.TextEffect, GameServerTextEffectMessage.Read);
        registry.Register(GameServerOpcode.MissleEffect, GameServerMissleEffectMessage.Read);
        registry.Register(GameServerOpcode.ClearTarget, GameServerClearTargetMessage.Read);
        registry.Register(GameServerOpcode.TutorialHint, GameServerTutorialHintMessage.Read);
        registry.Register(GameServerOpcode.AutomapFlag, GameServerAutomapFlagMessage.Read);
    }
}
