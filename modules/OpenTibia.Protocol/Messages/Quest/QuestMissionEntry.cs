// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Quest;

/// <summary>
/// One mission entry within a <see cref="GameServerQuestLineMessage" />.
/// </summary>
public sealed class QuestMissionEntry
{
    public QuestMissionEntry(string name, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    public string Name { get; }

    public string Description { get; }

    public static QuestMissionEntry Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var name = reader.ReadString();
        var description = reader.ReadString();
        return new QuestMissionEntry(name, description);
    }
}
