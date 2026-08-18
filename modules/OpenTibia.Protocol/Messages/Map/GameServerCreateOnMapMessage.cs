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
/// Server notice that an item or creature was added to a tile (s2c 0x6A).
/// </summary>
public sealed class GameServerCreateOnMapMessage : IProtocolMessage
{
    public GameServerCreateOnMapMessage(Position position, byte stackPosition, TileThing thing)
    {
        Position = position;
        StackPosition = stackPosition;
        Thing = thing;
    }

    public byte Opcode => (byte)GameServerOpcode.CreateOnMap;

    public Position Position { get; }

    public byte StackPosition { get; }

    public TileThing Thing { get; }

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
        var stackPosition = reader.ReadByte();
        var thing = TileThingCodec.Read(reader, itemTypes);
        return new GameServerCreateOnMapMessage(position, stackPosition, thing);
    }
}
