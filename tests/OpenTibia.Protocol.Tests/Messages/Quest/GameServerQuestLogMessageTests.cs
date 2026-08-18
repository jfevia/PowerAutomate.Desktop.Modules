// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Quest;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Quest;

[TestFixture]
public class GameServerQuestLogMessageTests
{
    [Test]
    public void Read_WithQuests_ParsesU16CountThenEveryEntry()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(2);
        WriteQuest(writer, 1, "Quest One", true);
        WriteQuest(writer, 2, "Quest Two", false);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerQuestLogMessage)GameServerQuestLogMessage.Read(GameServerOpcode.QuestLog, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.QuestLog));
            Assert.That(message.Quests, Has.Count.EqualTo(2));
            Assert.That(message.Quests[0].Name, Is.EqualTo("Quest One"));
            Assert.That(message.Quests[1].IsCompleted, Is.False);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNoQuests_DecodesEmptyList()
    {
        var reader = new PacketReader(new byte[] { 0, 0 });

        var message = (GameServerQuestLogMessage)GameServerQuestLogMessage.Read(GameServerOpcode.QuestLog, reader);

        Assert.That(message.Quests, Is.Empty);
    }

    [Test]
    public void Constructor_WithNullQuests_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerQuestLogMessage(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerQuestLogMessage.Read(GameServerOpcode.QuestLog, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 0 });

        Assert.Throws<ProtocolException>(() => GameServerQuestLogMessage.Read(GameServerOpcode.QuestLog, reader));
    }

    private static void WriteQuest(PacketWriter writer, ushort questId, string name, bool isCompleted)
    {
        writer.WriteUInt16(questId);
        writer.WriteString(name);
        writer.WriteByte((byte)(isCompleted ? 1 : 0));
    }
}
