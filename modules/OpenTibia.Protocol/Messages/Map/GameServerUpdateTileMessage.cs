// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// Server notice refreshing a specific tile's contents (s2c 0x69). A null <see cref="Tile" />
/// means the tile no longer exists.
/// </summary>
public sealed class GameServerUpdateTileMessage : IProtocolMessage
{
    private const int RemovedTileFlag = 1;

    public GameServerUpdateTileMessage(Position position, MapTile? tile)
    {
        Position = position;
        Tile = tile;
    }

    public byte Opcode => (byte)GameServerOpcode.UpdateTile;

    public Position Position { get; }

    public MapTile? Tile { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader, IItemTypeProvider itemTypes)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        var position = PositionCodec.Read(reader);
        var things = MapDescriptionCodec.ReadTileThings(reader, itemTypes, out var terminatorByte);
        if (things.Count == 0 && terminatorByte == RemovedTileFlag)
        {
            return new GameServerUpdateTileMessage(position, null);
        }

        return new GameServerUpdateTileMessage(position, new MapTile(things));
    }
}
