// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.ItemActions;

/// <summary>
/// Client request to move or throw an item from one position to another (opcode 0x78).
/// </summary>
public sealed class ClientMoveMessage : IClientMessage
{
    public ClientMoveMessage(Position fromPosition, ushort itemId, byte fromStackPosition, Position toPosition, byte count)
    {
        FromPosition = fromPosition;
        ItemId = itemId;
        FromStackPosition = fromStackPosition;
        ToPosition = toPosition;
        Count = count;
    }

    public Position FromPosition { get; }

    public ushort ItemId { get; }

    public byte FromStackPosition { get; }

    public Position ToPosition { get; }

    /// <summary>
    /// How many units of the item to move.
    /// </summary>
    public byte Count { get; }

    public byte Opcode => (byte)ClientOpcode.Move;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        PositionCodec.Write(writer, FromPosition);
        writer.WriteUInt16(ItemId);
        writer.WriteByte(FromStackPosition);
        PositionCodec.Write(writer, ToPosition);
        writer.WriteByte(Count);
    }
}
