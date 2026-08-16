// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

/// <summary>
/// Builds compact multi-floor payloads by chaining skip-run terminators between message tests.
/// </summary>
internal static class MapPayloadWriter
{
    public static void WriteTerminator(PacketWriter writer, int skip)
    {
        writer.WriteUInt16((ushort)(0xFF00 | skip));
    }

    /// <summary>
    /// Writes one real item tile, then chains 0xFF-capped skip runs to cover the remaining tiles.
    /// </summary>
    /// <param name="itemId">Must be 100, 200, or 300 so <see cref="FakeItemTypeProvider" /> expects the extra byte written here.</param>
    public static void WriteSingleTileFloors(PacketWriter writer, int totalTiles, ushort itemId, byte extra)
    {
        writer.WriteUInt16(itemId);
        writer.WriteByte(extra);

        var remaining = totalTiles - 1;
        while (remaining > 0)
        {
            var chunk = Math.Min(255, remaining);
            WriteTerminator(writer, chunk);
            remaining -= chunk;
        }
    }
}
