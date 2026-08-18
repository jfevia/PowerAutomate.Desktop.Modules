// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Items;

/// <summary>
/// The wire-relevant flags of one client thing.
/// </summary>
public sealed class DatItemFlags
{
    public DatItemFlags(bool isStackable, bool isFluidContainer, bool isSplash)
    {
        IsStackable = isStackable;
        IsFluidContainer = isFluidContainer;
        IsSplash = isSplash;
    }

    public bool IsStackable { get; }

    public bool IsFluidContainer { get; }

    public bool IsSplash { get; }

    /// <summary>
    /// True when the server writes a trailing count or subtype byte after the item id.
    /// </summary>
    public bool HasSubTypeByte => IsStackable || IsFluidContainer || IsSplash;
}

/// <summary>
/// Reads a protocol 8.60 Tibia.dat and classifies items by the client id the wire carries.
/// </summary>
/// <remarks>
/// Layout and attribute ids follow otclient's ThingType::unserialize, which applies no attribute
/// remapping for 8.6 to 9.86. Do not reuse this reader for other protocol versions.
/// </remarks>
public sealed class DatItemDatabase : IItemTypeProvider
{
    /// <summary>
    /// Client thing ids below this are not items; the item table starts here.
    /// </summary>
    public const ushort FirstItemId = 100;

    private const byte AttributeGround = 0;
    private const byte AttributeStackable = 5;
    private const byte AttributeWritable = 8;
    private const byte AttributeWritableOnce = 9;
    private const byte AttributeFluidContainer = 10;
    private const byte AttributeSplash = 11;
    private const byte AttributeLight = 21;
    private const byte AttributeDisplacement = 24;
    private const byte AttributeElevation = 25;
    private const byte AttributeMinimapColor = 28;
    private const byte AttributeLensHelp = 29;
    private const byte AttributeCloth = 32;
    private const byte AttributeMarket = 33;
    private const byte AttributeDefaultAction = 251;
    private const byte AttributeLast = 255;

    private readonly Dictionary<ushort, DatItemFlags> _itemsByClientId;

    private DatItemDatabase(Dictionary<ushort, DatItemFlags> itemsByClientId, uint signature)
    {
        _itemsByClientId = itemsByClientId;
        Signature = signature;
    }

    /// <summary>
    /// The .dat signature, which identifies the exact client build.
    /// </summary>
    public uint Signature { get; }

    public int Count => _itemsByClientId.Count;

    public int StackableCount => CountWhere(flags => flags.IsStackable);

    public int FluidContainerCount => CountWhere(flags => flags.IsFluidContainer);

    public int SplashCount => CountWhere(flags => flags.IsSplash);

    public static DatItemDatabase Load(string path)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        return Parse(File.ReadAllBytes(path));
    }

    public static DatItemDatabase Parse(byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var reader = new DatReader(data);
        var signature = reader.ReadUInt32();
        var itemCount = reader.ReadUInt16();
        reader.ReadUInt16();
        reader.ReadUInt16();
        reader.ReadUInt16();

        if (itemCount < FirstItemId)
        {
            throw new InvalidDataException(
                $"Not a recognizable Tibia.dat: it declares {itemCount} things, fewer than the first item id {FirstItemId}.");
        }

        var items = new Dictionary<ushort, DatItemFlags>(itemCount - FirstItemId + 1);
        for (var clientId = FirstItemId; clientId <= itemCount; clientId++)
        {
            items[clientId] = ReadThing(reader, clientId);
        }

        return new DatItemDatabase(items, signature);
    }

    public bool IsStackable(ushort itemId) => Lookup(itemId)?.IsStackable ?? false;

    public bool IsFluidContainer(ushort itemId) => Lookup(itemId)?.IsFluidContainer ?? false;

    public bool IsSplash(ushort itemId) => Lookup(itemId)?.IsSplash ?? false;

    /// <summary>
    /// The flags of one item, or null when the .dat does not describe that client id.
    /// </summary>
    public DatItemFlags? Lookup(ushort itemId)
    {
        return _itemsByClientId.TryGetValue(itemId, out var flags) ? flags : null;
    }

    private int CountWhere(Func<DatItemFlags, bool> predicate)
    {
        var total = 0;
        foreach (var flags in _itemsByClientId.Values)
        {
            if (predicate(flags))
            {
                total++;
            }
        }

        return total;
    }

    private static DatItemFlags ReadThing(DatReader reader, ushort clientId)
    {
        var isStackable = false;
        var isFluidContainer = false;
        var isSplash = false;

        while (true)
        {
            var attribute = reader.ReadByte();
            if (attribute == AttributeLast)
            {
                break;
            }

            switch (attribute)
            {
                case AttributeStackable:
                    isStackable = true;
                    break;
                case AttributeFluidContainer:
                    isFluidContainer = true;
                    break;
                case AttributeSplash:
                    isSplash = true;
                    break;
                case AttributeGround:
                case AttributeWritable:
                case AttributeWritableOnce:
                case AttributeElevation:
                case AttributeMinimapColor:
                case AttributeLensHelp:
                case AttributeCloth:
                case AttributeDefaultAction:
                    reader.Skip(2);
                    break;
                case AttributeLight:
                case AttributeDisplacement:
                    reader.Skip(4);
                    break;
                case AttributeMarket:
                    reader.Skip(6);
                    reader.SkipString();
                    reader.Skip(4);
                    break;
            }
        }

        SkipSprites(reader, clientId);
        return new DatItemFlags(isStackable, isFluidContainer, isSplash);
    }

    /// <summary>
    /// Consumes the sprite block so the reader lands on the next thing's first attribute.
    /// </summary>
    private static void SkipSprites(DatReader reader, ushort clientId)
    {
        var width = reader.ReadByte();
        var height = reader.ReadByte();
        if (width > 1 || height > 1)
        {
            reader.Skip(1);
        }

        var layers = reader.ReadByte();
        var patternX = reader.ReadByte();
        var patternY = reader.ReadByte();
        var patternZ = reader.ReadByte();
        var phases = reader.ReadByte();

        var total = (long)width * height * layers * patternX * patternY * patternZ * phases * 2;
        if (total > int.MaxValue)
        {
            throw new InvalidDataException($"Thing {clientId} declares an impossible sprite count.");
        }

        reader.Skip((int)total);
    }
}

/// <summary>
/// A bounds-checked forward cursor over .dat bytes.
/// </summary>
internal sealed class DatReader
{
    private readonly byte[] _data;
    private int _position;

    public DatReader(byte[] data)
    {
        _data = data;
    }

    public byte ReadByte()
    {
        Require(1);
        return _data[_position++];
    }

    public ushort ReadUInt16()
    {
        Require(2);
        var value = (ushort)(_data[_position] | (_data[_position + 1] << 8));
        _position += 2;
        return value;
    }

    public uint ReadUInt32()
    {
        Require(4);
        var value = (uint)(_data[_position]
                           | (_data[_position + 1] << 8)
                           | (_data[_position + 2] << 16)
                           | (_data[_position + 3] << 24));
        _position += 4;
        return value;
    }

    public void Skip(int count)
    {
        Require(count);
        _position += count;
    }

    public void SkipString()
    {
        Skip(ReadUInt16());
    }

    private void Require(int count)
    {
        if (_position + count > _data.Length)
        {
            throw new InvalidDataException(
                $"Truncated Tibia.dat: needed {count} byte(s) at offset {_position} but only {_data.Length - _position} remain.");
        }
    }
}
