// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Server notice of a creature's updated movement speed (s2c 0x8F).
/// </summary>
public sealed class GameServerCreatureSpeedMessage : IProtocolMessage
{
    public GameServerCreatureSpeedMessage(uint creatureId, ushort speed)
    {
        CreatureId = creatureId;
        Speed = speed;
    }

    public byte Opcode => (byte)GameServerOpcode.CreatureSpeed;

    public uint CreatureId { get; }

    public ushort Speed { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var creatureId = reader.ReadUInt32();
        var speed = reader.ReadUInt16();
        return new GameServerCreatureSpeedMessage(creatureId, speed);
    }
}
