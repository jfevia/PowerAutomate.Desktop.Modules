// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;

/// <summary>
/// One item and its count the local player can sell to an NPC.
/// </summary>
public sealed class GoodsEntry
{
    public GoodsEntry(ushort itemId, byte count)
    {
        ItemId = itemId;
        Count = count;
    }

    public ushort ItemId { get; }

    public byte Count { get; }

    public static GoodsEntry Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var itemId = reader.ReadUInt16();
        var count = reader.ReadByte();
        return new GoodsEntry(itemId, count);
    }
}
