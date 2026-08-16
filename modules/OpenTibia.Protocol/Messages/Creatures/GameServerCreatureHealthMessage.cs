// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Server notice of a creature's current health percentage (s2c 0x8C).
/// </summary>
public sealed class GameServerCreatureHealthMessage : IProtocolMessage
{
    public GameServerCreatureHealthMessage(uint creatureId, byte healthPercent)
    {
        CreatureId = creatureId;
        HealthPercent = healthPercent;
    }

    public byte Opcode => (byte)GameServerOpcode.CreatureHealth;

    public uint CreatureId { get; }

    public byte HealthPercent { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var creatureId = reader.ReadUInt32();
        var healthPercent = reader.ReadByte();
        return new GameServerCreatureHealthMessage(creatureId, healthPercent);
    }
}
