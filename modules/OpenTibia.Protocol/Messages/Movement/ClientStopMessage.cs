// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

/// <summary>
/// Client request to cancel the current auto-walk (opcode 0x69).
/// </summary>
public sealed class ClientStopMessage : IClientMessage
{
    public byte Opcode => (byte)ClientOpcode.Stop;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
    }
}
