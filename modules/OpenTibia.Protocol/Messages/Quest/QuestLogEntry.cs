// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Quest;

/// <summary>
/// One quest entry within a <see cref="GameServerQuestLogMessage" />.
/// </summary>
public sealed class QuestLogEntry
{
    public QuestLogEntry(ushort questId, string name, bool isCompleted)
    {
        QuestId = questId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsCompleted = isCompleted;
    }

    public ushort QuestId { get; }

    public string Name { get; }

    public bool IsCompleted { get; }

    public static QuestLogEntry Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var questId = reader.ReadUInt16();
        var name = reader.ReadString();
        var isCompleted = reader.ReadByte() != 0;
        return new QuestLogEntry(questId, name, isCompleted);
    }
}
