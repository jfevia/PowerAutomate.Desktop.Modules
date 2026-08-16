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
/// Server reveal of the map column now visible to the east after a step (s2c 0x66).
/// </summary>
public sealed class GameServerMapRightRowMessage : IProtocolMessage
{
    private const int FloorHeight = 14;

    public GameServerMapRightRowMessage(IReadOnlyList<IReadOnlyList<MapTile?>> floors)
    {
        Floors = floors;
    }

    public byte Opcode => (byte)GameServerOpcode.MapRightRow;

    public IReadOnlyList<IReadOnlyList<MapTile?>> Floors { get; }

    /// <summary>
    /// The wire doesn't encode the player's floor, so the caller must supply the tracked <paramref name="currentZ" />.
    /// </summary>
    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader, IItemTypeProvider itemTypes, byte currentZ)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        var floors = MapDescriptionCodec.Read(reader, itemTypes, currentZ, 1, FloorHeight);
        return new GameServerMapRightRowMessage(floors);
    }
}
