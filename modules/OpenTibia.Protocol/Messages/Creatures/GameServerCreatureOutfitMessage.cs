// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Server notice of a creature's updated outfit (s2c 0x8E).
/// </summary>
public sealed class GameServerCreatureOutfitMessage : IProtocolMessage
{
    public GameServerCreatureOutfitMessage(uint creatureId, OutfitDescriptor outfit)
    {
        CreatureId = creatureId;
        Outfit = outfit;
    }

    public byte Opcode => (byte)GameServerOpcode.CreatureOutfit;

    public uint CreatureId { get; }

    public OutfitDescriptor Outfit { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var creatureId = reader.ReadUInt32();
        var outfit = OutfitDescriptorCodec.Read(reader);
        return new GameServerCreatureOutfitMessage(creatureId, outfit);
    }
}
