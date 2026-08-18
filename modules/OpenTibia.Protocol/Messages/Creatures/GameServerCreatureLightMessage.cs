// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Server notice of a creature's current light source (s2c 0x8D).
/// </summary>
public sealed class GameServerCreatureLightMessage : IProtocolMessage
{
    public GameServerCreatureLightMessage(uint creatureId, LightInfo light)
    {
        CreatureId = creatureId;
        Light = light;
    }

    public byte Opcode => (byte)GameServerOpcode.CreatureLight;

    public uint CreatureId { get; }

    public LightInfo Light { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var creatureId = reader.ReadUInt32();
        var light = LightInfo.Read(reader);
        return new GameServerCreatureLightMessage(creatureId, light);
    }
}
