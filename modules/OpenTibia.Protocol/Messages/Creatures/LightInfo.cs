// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;

/// <summary>
/// A light source's level and color, matching TFS's LightInfo wire layout.
/// </summary>
public sealed class LightInfo
{
    public LightInfo(byte level, byte color)
    {
        Level = level;
        Color = color;
    }

    /// <summary>
    /// The light's intensity level (0 = none, 0xFF = full/ambient-override).
    /// </summary>
    public byte Level { get; }

    /// <summary>
    /// The light's color (a Tibia palette index).
    /// </summary>
    public byte Color { get; }

    public static LightInfo Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var level = reader.ReadByte();
        var color = reader.ReadByte();
        return new LightInfo(level, color);
    }
}
