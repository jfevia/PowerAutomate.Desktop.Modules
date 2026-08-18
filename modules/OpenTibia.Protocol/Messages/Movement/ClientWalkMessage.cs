// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

/// <summary>
/// Client request to take a single step (opcode depends on direction).
/// </summary>
public sealed class ClientWalkMessage : IClientMessage
{
    public ClientWalkMessage(Direction direction)
    {
        Direction = direction;
    }

    public Direction Direction { get; }

    public byte Opcode => (byte)DirectionOpcodes.ToWalkOpcode(Direction);

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
    }
}
