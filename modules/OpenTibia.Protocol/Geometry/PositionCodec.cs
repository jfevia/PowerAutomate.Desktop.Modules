// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;

/// <summary>
/// Reads and writes a <see cref="Position" /> as two little-endian words and a floor byte.
/// </summary>
public static class PositionCodec
{
    public static Position Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        var z = reader.ReadByte();
        return new Position(x, y, z);
    }

    public static void Write(PacketWriter writer, Position position)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        writer.WriteUInt16(position.X);
        writer.WriteUInt16(position.Y);
        writer.WriteByte(position.Z);
    }
}
