// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Quest;

/// <summary>
/// Registers the quest log and quest line message readers.
/// </summary>
public static class QuestMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(GameServerOpcode.QuestLog, GameServerQuestLogMessage.Read);
        registry.Register(GameServerOpcode.QuestLine, GameServerQuestLineMessage.Read);
    }
}
