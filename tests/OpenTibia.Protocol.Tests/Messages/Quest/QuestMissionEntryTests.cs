// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Quest;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Quest;

[TestFixture]
public class QuestMissionEntryTests
{
    [Test]
    public void Read_ParsesNameThenDescription()
    {
        var writer = new PacketWriter();
        writer.WriteString("Find the artifact");
        writer.WriteString("Search the ruins to the north.");
        var reader = new PacketReader(writer.ToArray());

        var entry = QuestMissionEntry.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(entry.Name, Is.EqualTo("Find the artifact"));
            Assert.That(entry.Description, Is.EqualTo("Search the ruins to the north."));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new QuestMissionEntry(null!, string.Empty));
    }

    [Test]
    public void Constructor_WithNullDescription_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new QuestMissionEntry(string.Empty, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => QuestMissionEntry.Read(null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        writer.WriteString("Find the artifact");
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => QuestMissionEntry.Read(reader));
    }
}
