// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// Server notice that the local player moved down one or more floors (s2c 0xBF). The wire format
/// doesn't encode how many pre-reveal floors follow, so the caller must supply that count.
/// </summary>
public sealed class GameServerFloorChangeDownMessage : IProtocolMessage
{
    private const int FloorWidth = 18;
    private const int FloorHeight = 14;

    public GameServerFloorChangeDownMessage(
        IReadOnlyList<IReadOnlyList<MapTile?>> preRevealFloors,
        IReadOnlyList<IReadOnlyList<MapTile?>> eastRow,
        IReadOnlyList<IReadOnlyList<MapTile?>> southRow)
    {
        PreRevealFloors = preRevealFloors;
        EastRow = eastRow;
        SouthRow = southRow;
    }

    public byte Opcode => (byte)GameServerOpcode.FloorChangeDown;

    /// <summary>
    /// Newly-visible floors directly below the player's new floor (0, 1, or 3 floors).
    /// </summary>
    public IReadOnlyList<IReadOnlyList<MapTile?>> PreRevealFloors { get; }

    public IReadOnlyList<IReadOnlyList<MapTile?>> EastRow { get; }

    public IReadOnlyList<IReadOnlyList<MapTile?>> SouthRow { get; }

    /// <summary>
    /// The wire doesn't encode <paramref name="preRevealFloorCount" /> or <paramref name="newZ" />; the caller must track both.
    /// </summary>
    public static IProtocolMessage Read(
        GameServerOpcode opcode, PacketReader reader, IItemTypeProvider itemTypes, int preRevealFloorCount, byte newZ)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        var preRevealFloors = MapDescriptionCodec.ReadRawFloors(reader, itemTypes, preRevealFloorCount, FloorWidth, FloorHeight);
        var eastRow = MapDescriptionCodec.Read(reader, itemTypes, newZ, 1, FloorHeight);
        var southRow = MapDescriptionCodec.Read(reader, itemTypes, newZ, FloorWidth, 1);
        return new GameServerFloorChangeDownMessage(preRevealFloors, eastRow, southRow);
    }
}
