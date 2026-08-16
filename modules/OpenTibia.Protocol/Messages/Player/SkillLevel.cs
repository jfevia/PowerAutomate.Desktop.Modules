// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Player;

/// <summary>
/// A single skill's level and progress, as reported in <see cref="GameServerPlayerSkillsMessage" />.
/// </summary>
public sealed class SkillLevel
{
    public SkillLevel(byte level, byte percent)
    {
        Level = level;
        Percent = percent;
    }

    public byte Level { get; }

    public byte Percent { get; }

    public static SkillLevel Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var level = reader.ReadByte();
        var percent = reader.ReadByte();
        return new SkillLevel(level, percent);
    }
}
