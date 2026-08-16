// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// Reads a <see cref="CreatureDescriptor" /> from a packet, matching TFS's AddCreature.
/// </summary>
public static class CreatureDescriptorCodec
{
    private const ushort KnownMarker = 0x0062;
    private const ushort UnknownMarker = 0x0061;

    /// <summary>
    /// Reads the known/unknown marker, id fields, health, direction, outfit, light, speed, and
    /// status markers.
    /// </summary>
    public static CreatureDescriptor Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var marker = reader.ReadUInt16();
        var isKnown = marker == KnownMarker;

        uint? removedCreatureId = null;
        string? name = null;
        uint creatureId;
        if (isKnown)
        {
            creatureId = reader.ReadUInt32();
        }
        else
        {
            removedCreatureId = reader.ReadUInt32();
            creatureId = reader.ReadUInt32();
            name = reader.ReadString();
        }

        var healthPercent = reader.ReadByte();
        var direction = (Direction)reader.ReadByte();
        var outfit = OutfitDescriptorCodec.Read(reader);
        var light = LightInfo.Read(reader);
        var speed = reader.ReadUInt16();
        var skull = reader.ReadByte();
        var shield = reader.ReadByte();

        byte? emblem = null;
        if (!isKnown)
        {
            emblem = reader.ReadByte();
        }

        var isUnpassable = reader.ReadByte() != 0;
        return new CreatureDescriptor(
            creatureId, isKnown, removedCreatureId, name, healthPercent, direction,
            outfit, light, speed, skull, shield, emblem, isUnpassable);
    }
}
