// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Quest;

/// <summary>
/// Server's mission details for one quest (s2c 0xF1).
/// </summary>
public sealed class GameServerQuestLineMessage : IProtocolMessage
{
    public GameServerQuestLineMessage(ushort questId, IReadOnlyList<QuestMissionEntry> missions)
    {
        QuestId = questId;
        Missions = missions ?? throw new ArgumentNullException(nameof(missions));
    }

    public byte Opcode => (byte)GameServerOpcode.QuestLine;

    public ushort QuestId { get; }

    public IReadOnlyList<QuestMissionEntry> Missions { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var questId = reader.ReadUInt16();
        var missionCount = reader.ReadByte();
        var missions = new List<QuestMissionEntry>(missionCount);
        for (var index = 0; index < missionCount; index++)
        {
            missions.Add(QuestMissionEntry.Read(reader));
        }

        return new GameServerQuestLineMessage(questId, missions);
    }
}
