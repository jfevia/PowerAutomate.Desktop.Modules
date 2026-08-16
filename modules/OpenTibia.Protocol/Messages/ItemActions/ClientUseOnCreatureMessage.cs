// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.ItemActions;

/// <summary>
/// Client request to use an item on a creature, e.g. a rune (opcode 0x84).
/// </summary>
public sealed class ClientUseOnCreatureMessage : IClientMessage
{
    public ClientUseOnCreatureMessage(Position position, ushort itemId, byte stackPosition, uint creatureId)
    {
        Position = position;
        ItemId = itemId;
        StackPosition = stackPosition;
        CreatureId = creatureId;
    }

    public Position Position { get; }

    public ushort ItemId { get; }

    public byte StackPosition { get; }

    public uint CreatureId { get; }

    public byte Opcode => (byte)ClientOpcode.UseOnCreature;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        PositionCodec.Write(writer, Position);
        writer.WriteUInt16(ItemId);
        writer.WriteByte(StackPosition);
        writer.WriteUInt32(CreatureId);
    }
}
