// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

/// <summary>
/// Client request to turn toward a cardinal direction without moving (opcode depends on direction).
/// </summary>
public sealed class ClientTurnMessage : IClientMessage
{
    public ClientTurnMessage(Direction direction)
    {
        Direction = direction;
    }

    public Direction Direction { get; }

    public byte Opcode => (byte)DirectionOpcodes.ToTurnOpcode(Direction);

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
    }
}
