// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// One selectable outfit within a <see cref="GameServerChooseOutfitMessage" />.
/// </summary>
public sealed class SelectableOutfit
{
    public SelectableOutfit(ushort lookType, string name, byte addons)
    {
        LookType = lookType;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Addons = addons;
    }

    /// <summary>
    /// The creature-sprite look type.
    /// </summary>
    public ushort LookType { get; }

    public string Name { get; }

    /// <summary>
    /// The addons bitmask the player is allowed to use for this outfit.
    /// </summary>
    public byte Addons { get; }

    public static SelectableOutfit Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var lookType = reader.ReadUInt16();
        var name = reader.ReadString();
        var addons = reader.ReadByte();
        return new SelectableOutfit(lookType, name, addons);
    }
}
