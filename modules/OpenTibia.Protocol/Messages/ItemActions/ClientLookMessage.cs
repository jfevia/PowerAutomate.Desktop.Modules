// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.ItemActions;

/// <summary>
/// Client request to look at an item (opcode 0x8C).
/// </summary>
public sealed class ClientLookMessage : IClientMessage
{
    public ClientLookMessage(Position position, ushort itemId, byte stackPosition)
    {
        Position = position;
        ItemId = itemId;
        StackPosition = stackPosition;
    }

    public Position Position { get; }

    public ushort ItemId { get; }

    public byte StackPosition { get; }

    public byte Opcode => (byte)ClientOpcode.Look;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        PositionCodec.Write(writer, Position);
        writer.WriteUInt16(ItemId);
        writer.WriteByte(StackPosition);
    }
}
