// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Server notice of whether a creature blocks pathing (s2c 0x92).
/// </summary>
public sealed class GameServerCreatureUnpassMessage : IProtocolMessage
{
    public GameServerCreatureUnpassMessage(uint creatureId, bool isUnpassable)
    {
        CreatureId = creatureId;
        IsUnpassable = isUnpassable;
    }

    public byte Opcode => (byte)GameServerOpcode.CreatureUnpass;

    public uint CreatureId { get; }

    public bool IsUnpassable { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var creatureId = reader.ReadUInt32();
        var isUnpassable = reader.ReadByte() != 0;
        return new GameServerCreatureUnpassMessage(creatureId, isUnpassable);
    }
}
