// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Player;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Social;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client.Handshake;

/// <summary>
/// Builds the opcode dispatch table for a game connection.
/// </summary>
public static class GameServerRegistryFactory
{
    /// <summary>
    /// TFS r3884 ProtocolGame::MoveUpCreature/MoveDownCreature key the pre-reveal count off the surface floor.
    /// </summary>
    private const byte GroundFloor = 7;

    /// <summary>
    /// The first floor below the surface.
    /// </summary>
    private const byte FirstUndergroundFloor = 8;

    /// <summary>
    /// The deepest floor that still reveals a single pre-drawn floor while descending.
    /// </summary>
    private const byte DeepestSteppedUndergroundFloor = 13;

    public static GameServerMessageRegistry CreateDefault()
    {
        return CreateDefault(new DefaultItemTypeProvider(), new MapFloorTracker());
    }

    /// <summary>
    /// Builds the registry with live floor tracking so the six floor-context map opcodes decode instead of throwing.
    /// </summary>
    public static GameServerMessageRegistry CreateDefault(IItemTypeProvider itemTypes, MapFloorTracker floors)
    {
        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        if (floors == null)
        {
            throw new ArgumentNullException(nameof(floors));
        }

        var registry = new GameServerMessageRegistry();

        LoginMessageReaders.RegisterTo(registry);
        PlayerMessageReaders.RegisterTo(registry);
        ChatMessageReaders.RegisterTo(registry);
        CreaturesMessageReaders.RegisterTo(registry);
        EffectsMessageReaders.RegisterTo(registry);
        SocialMessageReaders.RegisterTo(registry);
        MovementMessageReaders.RegisterTo(registry);
        MapMessageReaders.RegisterTo(registry, itemTypes);

        // Overwrites the six throwing registrations now that live floor context is available.
        registry.Register(GameServerOpcode.MapTopRow,
            (opcode, reader) => GameServerMapTopRowMessage.Read(opcode, reader, itemTypes, floors.CurrentZ));
        registry.Register(GameServerOpcode.MapRightRow,
            (opcode, reader) => GameServerMapRightRowMessage.Read(opcode, reader, itemTypes, floors.CurrentZ));
        registry.Register(GameServerOpcode.MapBottomRow,
            (opcode, reader) => GameServerMapBottomRowMessage.Read(opcode, reader, itemTypes, floors.CurrentZ));
        registry.Register(GameServerOpcode.MapLeftRow,
            (opcode, reader) => GameServerMapLeftRowMessage.Read(opcode, reader, itemTypes, floors.CurrentZ));
        registry.Register(GameServerOpcode.FloorChangeUp, (opcode, reader) =>
        {
            var newZ = (byte)(floors.CurrentZ - 1);
            return GameServerFloorChangeUpMessage.Read(opcode, reader, itemTypes, PreRevealFloorCountUp(newZ), newZ);
        });
        registry.Register(GameServerOpcode.FloorChangeDown, (opcode, reader) =>
        {
            var newZ = (byte)(floors.CurrentZ + 1);
            return GameServerFloorChangeDownMessage.Read(opcode, reader, itemTypes, PreRevealFloorCountDown(newZ), newZ);
        });

        // Ping carries no body and is answered by the reader loop, never by the flow.
        registry.Register(GameServerOpcode.Ping, (opcode, reader) => new PayloadlessMessage(opcode));

        return registry;
    }

    /// <summary>
    /// 6 floors reaching the surface, 1 while still underground, otherwise 0 (protocolgame.cpp r3884 MoveUpCreature).
    /// </summary>
    private static int PreRevealFloorCountUp(byte newZ)
    {
        if (newZ == GroundFloor)
        {
            return 6;
        }

        return newZ > GroundFloor ? 1 : 0;
    }

    /// <summary>
    /// 3 floors entering underground, 1 while still descending, otherwise 0 (protocolgame.cpp r3884 MoveDownCreature).
    /// </summary>
    private static int PreRevealFloorCountDown(byte newZ)
    {
        if (newZ == FirstUndergroundFloor)
        {
            return 3;
        }

        return newZ > FirstUndergroundFloor && newZ <= DeepestSteppedUndergroundFloor ? 1 : 0;
    }
}
