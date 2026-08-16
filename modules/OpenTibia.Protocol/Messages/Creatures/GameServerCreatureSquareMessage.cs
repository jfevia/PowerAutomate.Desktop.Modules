// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Server notice drawing a colored target-square marker around a creature (s2c 0x86).
/// </summary>
public sealed class GameServerCreatureSquareMessage : IProtocolMessage
{
    public GameServerCreatureSquareMessage(uint creatureId, TextColor color)
    {
        CreatureId = creatureId;
        Color = color;
    }

    public byte Opcode => (byte)GameServerOpcode.CreatureSquare;

    public uint CreatureId { get; }

    public TextColor Color { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var creatureId = reader.ReadUInt32();
        var color = (TextColor)reader.ReadByte();
        return new GameServerCreatureSquareMessage(creatureId, color);
    }
}
