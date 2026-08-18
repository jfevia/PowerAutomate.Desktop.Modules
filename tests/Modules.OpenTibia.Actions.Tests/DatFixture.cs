// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests;

/// <summary>
/// Builds synthetic Tibia.dat bytes, because the real client file is copyrighted.
/// </summary>
internal static class DatFixture
{
    private const byte AttributeStackable = 5;
    private const byte AttributeLast = 255;

    /// <summary>
    /// One thing per requested id, marked stackable when the id is listed.
    /// </summary>
    public static byte[] Build(ushort lastItemId, params ushort[] stackableIds)
    {
        var stackable = new HashSet<ushort>(stackableIds);
        var bytes = new List<byte> { 0x21, 0xB7, 0x28, 0x4C };
        AddUInt16(bytes, lastItemId);
        AddUInt16(bytes, 0);
        AddUInt16(bytes, 0);
        AddUInt16(bytes, 0);

        for (var id = DatItemDatabase.FirstItemId; id <= lastItemId; id++)
        {
            if (stackable.Contains(id))
            {
                bytes.Add(AttributeStackable);
            }

            bytes.Add(AttributeLast);

            // One 1x1 sprite, so the reader lands exactly on the next thing.
            bytes.AddRange(new byte[] { 1, 1, 1, 1, 1, 1, 1 });
            AddUInt16(bytes, id);
        }

        return bytes.ToArray();
    }

    public static TibiaItemDatabase CreateDatabase(ushort lastItemId = 110, params ushort[] stackableIds)
    {
        return new TibiaItemDatabase(DatItemDatabase.Parse(Build(lastItemId, stackableIds)), "Tibia.dat");
    }

    private static void AddUInt16(ICollection<byte> bytes, ushort value)
    {
        bytes.Add((byte)(value & 0xFF));
        bytes.Add((byte)(value >> 8));
    }
}
