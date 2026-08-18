// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Client request to follow a creature, or cancel when the id is zero (opcode 0xA2).
/// </summary>
public sealed class ClientFollowMessage : IClientMessage
{
    public ClientFollowMessage(uint creatureId, uint sequenceNumber)
    {
        CreatureId = creatureId;
        SequenceNumber = sequenceNumber;
    }

    public uint CreatureId { get; }

    /// <summary>
    /// Client-side request counter that 8.60 reads but ignores.
    /// </summary>
    public uint SequenceNumber { get; }

    public byte Opcode => (byte)ClientOpcode.Follow;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteUInt32(CreatureId);
        writer.WriteUInt32(SequenceNumber);
    }
}
