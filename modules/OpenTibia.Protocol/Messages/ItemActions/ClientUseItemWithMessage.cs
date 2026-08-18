// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.ItemActions;

/// <summary>
/// Client request to use one item together with another, e.g. a key on a door (opcode 0x83).
/// </summary>
public sealed class ClientUseItemWithMessage : IClientMessage
{
    public ClientUseItemWithMessage(
        Position fromPosition,
        ushort fromItemId,
        byte fromStackPosition,
        Position toPosition,
        ushort toItemId,
        byte toStackPosition)
    {
        FromPosition = fromPosition;
        FromItemId = fromItemId;
        FromStackPosition = fromStackPosition;
        ToPosition = toPosition;
        ToItemId = toItemId;
        ToStackPosition = toStackPosition;
    }

    public Position FromPosition { get; }

    public ushort FromItemId { get; }

    public byte FromStackPosition { get; }

    public Position ToPosition { get; }

    public ushort ToItemId { get; }

    public byte ToStackPosition { get; }

    public byte Opcode => (byte)ClientOpcode.UseItemWith;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        PositionCodec.Write(writer, FromPosition);
        writer.WriteUInt16(FromItemId);
        writer.WriteByte(FromStackPosition);
        PositionCodec.Write(writer, ToPosition);
        writer.WriteUInt16(ToItemId);
        writer.WriteByte(ToStackPosition);
    }
}
