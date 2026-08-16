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
public class GameServerQuestLineMessageTests
{
    [Test]
    public void Read_WithMissions_ParsesQuestIdThenEveryMission()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(7);
        writer.WriteByte(2);
        writer.WriteString("Mission One");
        writer.WriteString("Description One");
        writer.WriteString("Mission Two");
        writer.WriteString("Description Two");
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerQuestLineMessage)GameServerQuestLineMessage.Read(GameServerOpcode.QuestLine, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.QuestLine));
            Assert.That(message.QuestId, Is.EqualTo(7));
            Assert.That(message.Missions, Has.Count.EqualTo(2));
            Assert.That(message.Missions[0].Name, Is.EqualTo("Mission One"));
            Assert.That(message.Missions[1].Description, Is.EqualTo("Description Two"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNoMissions_DecodesEmptyList()
    {
        var reader = new PacketReader(new byte[] { 7, 0, 0 });

        var message = (GameServerQuestLineMessage)GameServerQuestLineMessage.Read(GameServerOpcode.QuestLine, reader);

        Assert.That(message.Missions, Is.Empty);
    }

    [Test]
    public void Constructor_WithNullMissions_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerQuestLineMessage(0, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerQuestLineMessage.Read(GameServerOpcode.QuestLine, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 7, 0 });

        Assert.Throws<ProtocolException>(() => GameServerQuestLineMessage.Read(GameServerOpcode.QuestLine, reader));
    }
}
