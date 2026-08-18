// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

/// <summary>
/// Reads a multi-floor map description: floor slices sharing one skip-run counter, matching TFS's GetMapDescription.
/// </summary>
public static class MapDescriptionCodec
{
    private const int AwarenessFloorRange = 2;
    private const int MaxFloorIndex = 15;
    private const int SeaFloor = 7;
    private const ushort SkipMarkerMask = 0xFF00;

    /// <summary>
    /// Reads every floor visible around a center floor: 5 floors nearby when underground, or the 8 surface floors down to ground level.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<MapTile?>> Read(
        PacketReader reader, IItemTypeProvider itemTypes, byte centerZ, int width, int height)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        var floorCount = centerZ > SeaFloor
            ? Math.Min(MaxFloorIndex, centerZ + AwarenessFloorRange) - (centerZ - AwarenessFloorRange) + 1
            : SeaFloor + 1;

        return ReadFloors(reader, itemTypes, width, height, floorCount);
    }

    /// <summary>
    /// Reads a fixed number of consecutive floor slices, used for a floor-change's pre-reveal floors.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<MapTile?>> ReadRawFloors(
        PacketReader reader, IItemTypeProvider itemTypes, int floorCount, int width, int height)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        return ReadFloors(reader, itemTypes, width, height, floorCount);
    }

    /// <summary>
    /// Reads one tile's stack until the skip-run terminator, honoring the shared creature/item/terminator marker space.
    /// </summary>
    internal static List<TileThing> ReadTileThings(PacketReader reader, IItemTypeProvider itemTypes, out int terminatorByte)
    {
        var things = new List<TileThing>();
        while (true)
        {
            if (TileThingCodec.HasCreatureMarker(reader))
            {
                things.Add(TileThingCodec.Read(reader, itemTypes));
                continue;
            }

            var value = reader.ReadUInt16();
            if ((value & SkipMarkerMask) == SkipMarkerMask)
            {
                terminatorByte = value & 0xFF;
                return things;
            }

            // The 2 bytes just read are the item id, so the item is built directly instead of re-read.
            var extra = itemTypes.IsStackable(value) || itemTypes.IsFluidContainer(value) || itemTypes.IsSplash(value)
                ? reader.ReadByte()
                : (byte)0;
            things.Add(new TileThing(new ItemStack(value, extra), null));
        }
    }

    private static List<IReadOnlyList<MapTile?>> ReadFloors(
        PacketReader reader, IItemTypeProvider itemTypes, int width, int height, int floorCount)
    {
        var skip = 0;
        var floors = new List<IReadOnlyList<MapTile?>>(floorCount);
        for (var i = 0; i < floorCount; i++)
        {
            floors.Add(ReadFloorSlice(reader, itemTypes, width, height, ref skip));
        }

        return floors;
    }

    private static List<MapTile?> ReadFloorSlice(
        PacketReader reader, IItemTypeProvider itemTypes, int width, int height, ref int skip)
    {
        var tiles = new List<MapTile?>(width * height);
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (skip > 0)
                {
                    tiles.Add(null);
                    skip--;
                    continue;
                }

                var things = ReadTileThings(reader, itemTypes, out var terminatorByte);
                tiles.Add(new MapTile(things));
                skip = terminatorByte;
            }
        }

        return tiles;
    }
}
