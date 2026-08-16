// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

/// <summary>
/// A message whose opcode carries no body.
/// </summary>
public sealed class PayloadlessMessage : IClientMessage
{
    public PayloadlessMessage(byte opcode)
    {
        Opcode = opcode;
    }

    public PayloadlessMessage(ClientOpcode opcode) : this((byte)opcode)
    {
    }

    public PayloadlessMessage(GameServerOpcode opcode) : this((byte)opcode)
    {
    }

    public byte Opcode { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
    }

    public override string ToString()
    {
        return $"opcode 0x{Opcode:X2}";
    }
}
