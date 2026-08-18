// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Quest;

/// <summary>
/// Server's list of every started quest and its completion state (s2c 0xF0).
/// </summary>
public sealed class GameServerQuestLogMessage : IProtocolMessage
{
    public GameServerQuestLogMessage(IReadOnlyList<QuestLogEntry> quests)
    {
        Quests = quests ?? throw new ArgumentNullException(nameof(quests));
    }

    public byte Opcode => (byte)GameServerOpcode.QuestLog;

    public IReadOnlyList<QuestLogEntry> Quests { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var questCount = reader.ReadUInt16();
        var quests = new List<QuestLogEntry>(questCount);
        for (var index = 0; index < questCount; index++)
        {
            quests.Add(QuestLogEntry.Read(reader));
        }

        return new GameServerQuestLogMessage(quests);
    }
}
