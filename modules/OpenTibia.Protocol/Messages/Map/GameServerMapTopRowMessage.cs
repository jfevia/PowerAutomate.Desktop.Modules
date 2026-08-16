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
/// Server reveal of the map row now visible to the north after a step (s2c 0x65).
/// </summary>
public sealed class GameServerMapTopRowMessage : IProtocolMessage
{
    private const int FloorWidth = 18;

    public GameServerMapTopRowMessage(IReadOnlyList<IReadOnlyList<MapTile?>> floors)
    {
        Floors = floors;
    }

    public byte Opcode => (byte)GameServerOpcode.MapTopRow;

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

        var floors = MapDescriptionCodec.Read(reader, itemTypes, currentZ, FloorWidth, 1);
        return new GameServerMapTopRowMessage(floors);
    }
}
