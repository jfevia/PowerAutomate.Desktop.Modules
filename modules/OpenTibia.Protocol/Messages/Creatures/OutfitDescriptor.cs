// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// A creature's appearance, matching TFS's AddCreatureOutfit wire layout.
/// </summary>
public sealed class OutfitDescriptor
{
    public OutfitDescriptor(
        ushort lookType,
        byte? head,
        byte? body,
        byte? legs,
        byte? feet,
        byte? addons,
        ushort? lookTypeEx)
    {
        LookType = lookType;
        Head = head;
        Body = body;
        Legs = legs;
        Feet = feet;
        Addons = addons;
        LookTypeEx = lookTypeEx;
    }

    /// <summary>
    /// The creature-sprite look type, or 0 for an item disguise/invisible creature.
    /// </summary>
    public ushort LookType { get; }

    /// <summary>
    /// The head color, present only when <see cref="LookType" /> is nonzero.
    /// </summary>
    public byte? Head { get; }

    /// <summary>
    /// The body color, present only when <see cref="LookType" /> is nonzero.
    /// </summary>
    public byte? Body { get; }

    /// <summary>
    /// The legs color, present only when <see cref="LookType" /> is nonzero.
    /// </summary>
    public byte? Legs { get; }

    /// <summary>
    /// The feet color, present only when <see cref="LookType" /> is nonzero.
    /// </summary>
    public byte? Feet { get; }

    /// <summary>
    /// The addons bitmask, present only when <see cref="LookType" /> is nonzero.
    /// </summary>
    public byte? Addons { get; }

    /// <summary>
    /// The disguise item's client id, present only when <see cref="LookType" /> is zero.
    /// </summary>
    public ushort? LookTypeEx { get; }
}
