// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// A creature's full appearance and status, matching TFS's AddCreature wire layout. When
/// <see cref="IsKnown" /> is true, the client already has this creature cached.
/// </summary>
public sealed class CreatureDescriptor
{
    public CreatureDescriptor(
        uint creatureId,
        bool isKnown,
        uint? removedCreatureId,
        string? name,
        byte healthPercent,
        Direction direction,
        OutfitDescriptor outfit,
        LightInfo light,
        ushort speed,
        byte skull,
        byte shield,
        byte? emblem,
        bool isUnpassable)
    {
        CreatureId = creatureId;
        IsKnown = isKnown;
        RemovedCreatureId = removedCreatureId;
        Name = name;
        HealthPercent = healthPercent;
        Direction = direction;
        Outfit = outfit;
        Light = light;
        Speed = speed;
        Skull = skull;
        Shield = shield;
        Emblem = emblem;
        IsUnpassable = isUnpassable;
    }

    /// <summary>
    /// True if the local client already has this creature cached (only <see cref="CreatureId" />
    /// is meaningful on the wire in that case).
    /// </summary>
    public bool IsKnown { get; }

    /// <summary>
    /// The id of a previously-known creature to evict from the client cache, present only when
    /// not already known.
    /// </summary>
    public uint? RemovedCreatureId { get; }

    public uint CreatureId { get; }

    /// <summary>
    /// The creature's display name, present only when not already known.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Health as a percentage (0-100), or 0 if the creature hides its health.
    /// </summary>
    public byte HealthPercent { get; }

    public Direction Direction { get; }

    public OutfitDescriptor Outfit { get; }

    public LightInfo Light { get; }

    public ushort Speed { get; }

    /// <summary>
    /// The skull marker to display.
    /// </summary>
    public byte Skull { get; }

    /// <summary>
    /// The party shield marker to display.
    /// </summary>
    public byte Shield { get; }

    /// <summary>
    /// The guild emblem marker, present only when not already known.
    /// </summary>
    public byte? Emblem { get; }

    /// <summary>
    /// True if the local player cannot walk through this creature.
    /// </summary>
    public bool IsUnpassable { get; }
}
