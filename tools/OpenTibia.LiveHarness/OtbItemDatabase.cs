// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace OpenTibia.LiveHarness;

/// <summary>
/// An items.otb node group, matching TFS's itemgroup_t values.
/// </summary>
public enum OtbItemGroup : byte
{
    None = 0,
    Ground = 1,
    Container = 2,
    Charges = 6,
    Splash = 11,
    Fluid = 12
}

/// <summary>
/// One items.otb entry, keyed by the client (sprite) id the wire protocol actually carries.
/// </summary>
public sealed class OtbItemRecord
{
    public OtbItemRecord(ushort clientId, ushort serverId, OtbItemGroup group, bool isStackable)
    {
        ClientId = clientId;
        ServerId = serverId;
        Group = group;
        IsStackable = isStackable;
    }

    /// <summary>
    /// The ITEM_ATTR_CLIENTID value: the sprite id sent on the wire.
    /// </summary>
    public ushort ClientId { get; }

    /// <summary>
    /// The ITEM_ATTR_SERVERID value: the TFS-internal id used by items.xml and scripts.
    /// </summary>
    public ushort ServerId { get; }

    /// <summary>
    /// The OTB node group this item belongs to.
    /// </summary>
    public OtbItemGroup Group { get; }

    /// <summary>
    /// True if OTB flag bit 0x80 (FLAG_STACKABLE) is set.
    /// </summary>
    public bool IsStackable { get; }

    /// <summary>
    /// True if this item's group is <see cref="OtbItemGroup.Fluid" />.
    /// </summary>
    public bool IsFluidContainer => Group == OtbItemGroup.Fluid;

    /// <summary>
    /// True if this item's group is <see cref="OtbItemGroup.Splash" />.
    /// </summary>
    public bool IsSplash => Group == OtbItemGroup.Splash;
}

/// <summary>
/// Parses a TFS items.otb node tree into item records keyed by client id.
/// </summary>
public static class OtbItemDatabase
{
    private const byte AttributeServerId = 0x10;
    private const byte AttributeClientId = 0x11;
    private const byte NodeStart = 0xFE;
    private const byte NodeEnd = 0xFF;
    private const byte Escape = 0xFD;
    private const uint FlagStackable = 0x80;

    /// <summary>
    /// Reads and parses an items.otb file from disk.
    /// </summary>
    public static IReadOnlyDictionary<ushort, OtbItemRecord> Load(string path)
    {
        return Parse(File.ReadAllBytes(path));
    }

    /// <summary>
    /// Parses raw items.otb bytes into a lookup keyed by client id.
    /// </summary>
    public static IReadOnlyDictionary<ushort, OtbItemRecord> Parse(byte[] otbData)
    {
        if (otbData == null)
        {
            throw new ArgumentNullException(nameof(otbData));
        }

        if (otbData.Length < 5 || otbData[4] != NodeStart)
        {
            throw new InvalidDataException("Not a recognizable items.otb file (missing root node marker).");
        }

        var position = SkipRootProperties(otbData, 5);
        var itemsByClientId = new Dictionary<ushort, OtbItemRecord>();

        while (position < otbData.Length && otbData[position] == NodeStart)
        {
            position++;
            var nodeType = otbData[position];
            position++;

            var properties = ReadEscapedProperties(otbData, ref position);
            if (position >= otbData.Length || otbData[position] != NodeEnd)
            {
                throw new InvalidDataException("Malformed items.otb: expected a node-end marker.");
            }

            position++;

            var record = ParseItemNode(nodeType, properties);
            if (record != null)
            {
                itemsByClientId[record.ClientId] = record;
            }
        }

        return itemsByClientId;
    }

    private static OtbItemRecord? ParseItemNode(byte nodeType, byte[] properties)
    {
        if (properties.Length < 4)
        {
            return null;
        }

        var flags = ReadUInt32(properties, 0);
        var isStackable = (flags & FlagStackable) != 0;

        ushort? clientId = null;
        ushort serverId = 0;
        var offset = 4;
        while (offset + 3 <= properties.Length)
        {
            var attribute = properties[offset];
            var length = ReadUInt16(properties, offset + 1);
            var dataOffset = offset + 3;
            if (dataOffset + length > properties.Length)
            {
                break;
            }

            if (attribute == AttributeClientId && length == 2)
            {
                clientId = ReadUInt16(properties, dataOffset);
            }
            else if (attribute == AttributeServerId && length == 2)
            {
                serverId = ReadUInt16(properties, dataOffset);
            }

            offset = dataOffset + length;
        }

        if (clientId == null)
        {
            return null;
        }

        return new OtbItemRecord(clientId.Value, serverId, (OtbItemGroup)nodeType, isStackable);
    }

    /// <summary>
    /// Reads one node's property bytes, unescaping 0xFD, stopping at an unescaped node marker.
    /// </summary>
    private static byte[] ReadEscapedProperties(byte[] otbData, ref int position)
    {
        var buffer = new List<byte>();
        while (position < otbData.Length)
        {
            var current = otbData[position];
            if (current == NodeStart || current == NodeEnd)
            {
                break;
            }

            if (current == Escape)
            {
                position++;
                buffer.Add(otbData[position]);
                position++;
            }
            else
            {
                buffer.Add(current);
                position++;
            }
        }

        return buffer.ToArray();
    }

    /// <summary>
    /// Skips the root node's own type byte and properties to reach its first child node.
    /// </summary>
    private static int SkipRootProperties(byte[] otbData, int position)
    {
        while (position < otbData.Length)
        {
            var current = otbData[position];
            if (current == NodeStart || current == NodeEnd)
            {
                return position;
            }

            position += current == Escape ? 2 : 1;
        }

        throw new InvalidDataException("Malformed items.otb: root node properties never terminate.");
    }

    private static ushort ReadUInt16(byte[] buffer, int offset)
    {
        return (ushort)(buffer[offset] | (buffer[offset + 1] << 8));
    }

    private static uint ReadUInt32(byte[] buffer, int offset)
    {
        return (uint)(buffer[offset] | (buffer[offset + 1] << 8) | (buffer[offset + 2] << 16) | (buffer[offset + 3] << 24));
    }
}
