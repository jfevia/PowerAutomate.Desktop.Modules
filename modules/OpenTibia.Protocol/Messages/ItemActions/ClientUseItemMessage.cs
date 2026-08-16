// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.ItemActions;

/// <summary>
/// Client request to use an item at a position (opcode 0x82).
/// </summary>
public sealed class ClientUseItemMessage : IClientMessage
{
    public ClientUseItemMessage(Position position, ushort itemId, byte stackPosition, byte containerIndex)
    {
        Position = position;
        ItemId = itemId;
        StackPosition = stackPosition;
        ContainerIndex = containerIndex;
    }

    public Position Position { get; }

    public ushort ItemId { get; }

    public byte StackPosition { get; }

    /// <summary>
    /// The container/hotkey slot the use was invoked from.
    /// </summary>
    public byte ContainerIndex { get; }

    public byte Opcode => (byte)ClientOpcode.UseItem;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        PositionCodec.Write(writer, Position);
        writer.WriteUInt16(ItemId);
        writer.WriteByte(StackPosition);
        writer.WriteByte(ContainerIndex);
    }
}
