// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// Reads one tile-stack entry (an item or a creature) from a packet.
/// </summary>
public static class TileThingCodec
{
    private const byte UnknownCreatureMarker = 0x61;
    private const byte KnownCreatureMarker = 0x62;

    public static TileThing Read(PacketReader reader, IItemTypeProvider itemTypes)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        if (HasCreatureMarker(reader))
        {
            return new TileThing(null, CreatureDescriptorCodec.Read(reader));
        }

        return new TileThing(ItemStackCodec.Read(reader, itemTypes), null);
    }

    /// <summary>
    /// Peeks only the low byte, so an item id of 0x61/0x62 with a nonzero high byte misreads as a creature.
    /// </summary>
    internal static bool HasCreatureMarker(PacketReader reader)
    {
        var marker = reader.PeekByte();
        return marker == UnknownCreatureMarker || marker == KnownCreatureMarker;
    }
}
