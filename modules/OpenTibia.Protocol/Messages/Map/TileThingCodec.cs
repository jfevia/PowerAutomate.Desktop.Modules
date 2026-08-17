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
    private const ushort UnknownCreatureMarker = 0x0061;
    private const ushort KnownCreatureMarker = 0x0062;

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
    /// Compares the whole word, because item ids such as 0x0E61 share the low byte of a creature marker.
    /// </summary>
    public static bool HasCreatureMarker(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (reader.Remaining < 2)
        {
            return false;
        }

        var marker = reader.PeekUInt16();
        return marker == UnknownCreatureMarker || marker == KnownCreatureMarker;
    }
}
