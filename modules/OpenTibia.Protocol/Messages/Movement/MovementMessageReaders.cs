// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

/// <summary>
/// Registers the movement notice message readers.
/// </summary>
public static class MovementMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(GameServerOpcode.CancelWalk, GameServerCancelWalkMessage.Read);
        registry.Register(GameServerOpcode.MoveCreature, GameServerMoveCreatureMessage.Read);
    }
}
