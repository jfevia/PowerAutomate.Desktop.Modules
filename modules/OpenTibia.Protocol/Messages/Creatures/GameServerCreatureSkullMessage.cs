// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Server notice of a creature's skull marker (s2c 0x90).
/// </summary>
public sealed class GameServerCreatureSkullMessage : IProtocolMessage
{
    public GameServerCreatureSkullMessage(uint creatureId, byte skull)
    {
        CreatureId = creatureId;
        Skull = skull;
    }

    public byte Opcode => (byte)GameServerOpcode.CreatureSkull;

    public uint CreatureId { get; }

    public byte Skull { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var creatureId = reader.ReadUInt32();
        var skull = reader.ReadByte();
        return new GameServerCreatureSkullMessage(creatureId, skull);
    }
}
