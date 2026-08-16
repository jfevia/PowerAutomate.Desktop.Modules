// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Server notice of a creature's party shield marker (s2c 0x91).
/// </summary>
public sealed class GameServerCreatureShieldMessage : IProtocolMessage
{
    public GameServerCreatureShieldMessage(uint creatureId, byte shield)
    {
        CreatureId = creatureId;
        Shield = shield;
    }

    public byte Opcode => (byte)GameServerOpcode.CreatureParty;

    public uint CreatureId { get; }

    public byte Shield { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var creatureId = reader.ReadUInt32();
        var shield = reader.ReadByte();
        return new GameServerCreatureShieldMessage(creatureId, shield);
    }
}
