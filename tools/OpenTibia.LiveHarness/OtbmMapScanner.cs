// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace OpenTibia.LiveHarness;

/// <summary>
/// One map tile carrying an item that changes the walker's floor.
/// </summary>
public sealed class FloorChangeTile
{
    public FloorChangeTile(ushort x, ushort y, byte z, ushort itemId, string change, bool isGround)
    {
        X = x;
        Y = y;
        Z = z;
        ItemId = itemId;
        Change = change;
        IsGround = isGround;
    }

    public ushort X { get; }

    public ushort Y { get; }

    public byte Z { get; }

    /// <summary>
    /// The TFS server id, which is what an OTBM stores.
    /// </summary>
    public ushort ItemId { get; }

    /// <summary>
    /// The items.xml floorchange value, such as down, north or west.
    /// </summary>
    public string Change { get; }

    public bool IsGround { get; }

    public override string ToString()
    {
        return $"{X},{Y},{Z} item={ItemId} change={Change} ground={IsGround}";
    }
}

/// <summary>
/// Reads a TFS .otbm node tree looking for tiles that would move a walker between floors.
/// </summary>
/// <remarks>
/// Node ids and struct layouts follow this server's own iomap.h, not a generic OTBM description.
/// </remarks>
public static class OtbmMapScanner
{
    private const byte NodeStart = 0xFE;
    private const byte NodeEnd = 0xFF;
    private const byte Escape = 0xFD;

    private const byte NodeTileArea = 4;
    private const byte NodeTile = 5;
    private const byte NodeItem = 6;
    private const byte NodeHouseTile = 14;

    private const byte AttributeTileFlags = 3;
    private const byte AttributeItem = 9;

    /// <summary>
    /// Reads items.xml and returns every server id whose floorchange attribute is set.
    /// </summary>
    public static IReadOnlyDictionary<ushort, string> LoadFloorChangeIds(string itemsXmlPath)
    {
        var document = XDocument.Parse(File.ReadAllText(itemsXmlPath));
        var result = new Dictionary<ushort, string>();

        foreach (var item in document.Root!.Elements("item"))
        {
            var changes = item.Elements("attribute")
                .Where(attribute => string.Equals((string?)attribute.Attribute("key"), "floorchange", StringComparison.OrdinalIgnoreCase))
                .Select(attribute => (string?)attribute.Attribute("value") ?? string.Empty)
                .Where(value => value.Length > 0)
                .ToList();

            if (changes.Count == 0)
            {
                continue;
            }

            var change = string.Join("+", changes);
            foreach (var id in ExpandIds(item))
            {
                result[id] = change;
            }
        }

        return result;
    }

    /// <summary>
    /// Walks the whole map and returns the tiles holding one of the wanted item ids.
    /// </summary>
    public static IReadOnlyList<FloorChangeTile> FindTiles(string otbmPath, IReadOnlyDictionary<ushort, string> wanted)
    {
        var data = File.ReadAllBytes(otbmPath);
        if (data.Length < 6 || data[4] != NodeStart)
        {
            throw new InvalidDataException("Not a recognizable .otbm file (missing root node marker).");
        }

        var results = new List<FloorChangeTile>();
        var scratch = new byte[64 * 1024];
        var position = 5;
        var context = default(TileContext);
        ParseNode(data, ref position, ref scratch, wanted, results, context);
        return results;
    }

    private static void ParseNode(
        byte[] data,
        ref int position,
        ref byte[] scratch,
        IReadOnlyDictionary<ushort, string> wanted,
        ICollection<FloorChangeTile> results,
        TileContext context)
    {
        var nodeType = data[position];
        position++;

        var length = ReadEscapedProperties(data, ref position, ref scratch);

        switch (nodeType)
        {
            case NodeTileArea when length >= 5:
                context.BaseX = ReadUInt16(scratch, 0);
                context.BaseY = ReadUInt16(scratch, 2);
                context.Z = scratch[4];
                break;

            case NodeTile:
            case NodeHouseTile:
                ReadTile(nodeType, scratch, length, wanted, results, ref context);
                break;

            case NodeItem when length >= 2 && context.HasTile:
                var itemId = ReadUInt16(scratch, 0);
                if (wanted.TryGetValue(itemId, out var change))
                {
                    results.Add(new FloorChangeTile(context.TileX, context.TileY, context.Z, itemId, change, false));
                }

                break;
        }

        while (position < data.Length && data[position] == NodeStart)
        {
            position++;
            ParseNode(data, ref position, ref scratch, wanted, results, context);
        }

        if (position >= data.Length || data[position] != NodeEnd)
        {
            throw new InvalidDataException($"Malformed .otbm: expected a node-end marker at offset {position}.");
        }

        position++;
    }

    private static void ReadTile(
        byte nodeType,
        byte[] properties,
        int length,
        IReadOnlyDictionary<ushort, string> wanted,
        ICollection<FloorChangeTile> results,
        ref TileContext context)
    {
        if (length < 2)
        {
            return;
        }

        context.TileX = (ushort)(context.BaseX + properties[0]);
        context.TileY = (ushort)(context.BaseY + properties[1]);
        context.HasTile = true;

        // A house tile carries its house id before the attribute list.
        var offset = nodeType == NodeHouseTile ? 6 : 2;

        while (offset < length)
        {
            var attribute = properties[offset];
            offset++;

            if (attribute == AttributeTileFlags)
            {
                offset += 4;
                continue;
            }

            if (attribute == AttributeItem && offset + 2 <= length)
            {
                var groundId = ReadUInt16(properties, offset);
                offset += 2;
                if (wanted.TryGetValue(groundId, out var change))
                {
                    results.Add(new FloorChangeTile(context.TileX, context.TileY, context.Z, groundId, change, true));
                }

                continue;
            }

            // Anything else on a tile is unexpected, and guessing its size would corrupt the walk.
            return;
        }
    }

    private static IEnumerable<ushort> ExpandIds(XElement item)
    {
        var single = (string?)item.Attribute("id");
        if (single != null && ushort.TryParse(single, out var id))
        {
            yield return id;
            yield break;
        }

        var from = (string?)item.Attribute("fromid");
        var to = (string?)item.Attribute("toid");
        if (from == null || to == null || !ushort.TryParse(from, out var first) || !ushort.TryParse(to, out var last))
        {
            yield break;
        }

        for (var current = first; current <= last; current++)
        {
            yield return current;
        }
    }

    private static int ReadEscapedProperties(byte[] data, ref int position, ref byte[] scratch)
    {
        var length = 0;
        while (position < data.Length)
        {
            var current = data[position];
            if (current == NodeStart || current == NodeEnd)
            {
                break;
            }

            if (current == Escape)
            {
                position++;
                current = data[position];
            }

            if (length == scratch.Length)
            {
                Array.Resize(ref scratch, scratch.Length * 2);
            }

            scratch[length] = current;
            length++;
            position++;
        }

        return length;
    }

    private static ushort ReadUInt16(byte[] buffer, int offset)
    {
        return (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
    }

    private struct TileContext
    {
        public ushort BaseX;
        public ushort BaseY;
        public byte Z;
        public ushort TileX;
        public ushort TileY;
        public bool HasTile;
    }
}
