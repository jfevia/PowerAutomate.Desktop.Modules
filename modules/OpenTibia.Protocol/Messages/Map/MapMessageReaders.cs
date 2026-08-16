// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// Registers the map and tile update message readers.
/// </summary>
public static class MapMessageReaders
{
    /// <summary>
    /// Registers every opcode using <see cref="DefaultItemTypeProvider" />, since no items.otb ships with this module.
    /// </summary>
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        RegisterTo(registry, new DefaultItemTypeProvider());
    }

    public static void RegisterTo(GameServerMessageRegistry registry, IItemTypeProvider itemTypes)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        registry.Register(GameServerOpcode.FullMap, (opcode, reader) => GameServerFullMapMessage.Read(opcode, reader, itemTypes));
        registry.Register(GameServerOpcode.MapTopRow, (opcode, reader) => throw MissingFloorContext(opcode));
        registry.Register(GameServerOpcode.MapRightRow, (opcode, reader) => throw MissingFloorContext(opcode));
        registry.Register(GameServerOpcode.MapBottomRow, (opcode, reader) => throw MissingFloorContext(opcode));
        registry.Register(GameServerOpcode.MapLeftRow, (opcode, reader) => throw MissingFloorContext(opcode));
        registry.Register(GameServerOpcode.UpdateTile, (opcode, reader) => GameServerUpdateTileMessage.Read(opcode, reader, itemTypes));
        registry.Register(GameServerOpcode.CreateOnMap, (opcode, reader) => GameServerCreateOnMapMessage.Read(opcode, reader, itemTypes));
        registry.Register(GameServerOpcode.ChangeOnMap, (opcode, reader) => GameServerChangeOnMapMessage.Read(opcode, reader, itemTypes));
        registry.Register(GameServerOpcode.DeleteOnMap, GameServerDeleteOnMapMessage.Read);
        registry.Register(GameServerOpcode.FloorChangeUp, (opcode, reader) => throw MissingFloorContext(opcode));
        registry.Register(GameServerOpcode.FloorChangeDown, (opcode, reader) => throw MissingFloorContext(opcode));
    }

    /// <summary>
    /// The registry has no session state, so floor-context messages must be decoded via their own Read overload.
    /// </summary>
    private static NotSupportedException MissingFloorContext(GameServerOpcode opcode)
    {
        return new NotSupportedException(
            $"{opcode} needs caller-tracked floor context; call its Read overload directly instead of through the registry.");
    }
}
