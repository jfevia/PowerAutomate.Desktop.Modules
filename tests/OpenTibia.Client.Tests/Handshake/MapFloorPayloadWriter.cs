// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Handshake;

/// <summary>
/// Builds empty-tile map payloads whose skip runs never collide with the 0x61/0x62 creature markers.
/// </summary>
internal static class MapFloorPayloadWriter
{
    private const int AwarenessFloorRange = 2;
    private const int MaxFloorIndex = 15;
    private const int SeaFloor = 7;

    // Stays below the 0x61/0x62 creature-descriptor markers so a skip run is never misread as a creature.
    private const int MaxSkipChunk = 96;

    public const int FloorWidth = 18;
    public const int FloorHeight = 14;

    /// <summary>
    /// Mirrors MapDescriptionCodec's private floor-count rule for a given center floor.
    /// </summary>
    public static int FloorCountFor(byte centerZ)
    {
        return centerZ > SeaFloor
            ? Math.Min(MaxFloorIndex, centerZ + AwarenessFloorRange) - (centerZ - AwarenessFloorRange) + 1
            : SeaFloor + 1;
    }

    /// <summary>
    /// Writes enough skip-run terminators to represent <paramref name="totalTiles" /> entirely empty tiles.
    /// </summary>
    public static void WriteEmptyTiles(PacketWriter writer, int totalTiles)
    {
        var remaining = totalTiles;
        while (remaining > 0)
        {
            var chunk = Math.Min(MaxSkipChunk, remaining - 1);
            writer.WriteUInt16((ushort)(0xFF00 | chunk));
            remaining -= chunk + 1;
        }
    }
}
