// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// Server notice that a tile's item changed, or that a creature merely turned in place (s2c
/// 0x6B). Exactly one of <see cref="ChangedItem" /> or <see cref="TurnedCreatureId" /> is present.
/// </summary>
public sealed class GameServerChangeOnMapMessage : IProtocolMessage
{
    private const byte TurnMarkerLowByte = 0x63;

    public GameServerChangeOnMapMessage(
        Position position, byte stackPosition, ItemStack? changedItem, uint? turnedCreatureId, Direction? newDirection)
    {
        Position = position;
        StackPosition = stackPosition;
        ChangedItem = changedItem;
        TurnedCreatureId = turnedCreatureId;
        NewDirection = newDirection;
    }

    public byte Opcode => (byte)GameServerOpcode.ChangeOnMap;

    public Position Position { get; }

    public byte StackPosition { get; }

    public ItemStack? ChangedItem { get; }

    public uint? TurnedCreatureId { get; }

    public Direction? NewDirection { get; }

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

        // Peeking only the low byte, so an item id of 0x63 with a nonzero high byte misreads as a turn.
        if (reader.PeekByte() == TurnMarkerLowByte)
        {
            reader.Skip(2);
            var turnedCreatureId = reader.ReadUInt32();
            var newDirection = (Direction)reader.ReadByte();
            return new GameServerChangeOnMapMessage(position, stackPosition, null, turnedCreatureId, newDirection);
        }

        var changedItem = ItemStackCodec.Read(reader, itemTypes);
        return new GameServerChangeOnMapMessage(position, stackPosition, changedItem, null, null);
    }
}
