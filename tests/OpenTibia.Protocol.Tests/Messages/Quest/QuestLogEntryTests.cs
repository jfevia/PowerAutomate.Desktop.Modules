// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Quest;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Quest;

[TestFixture]
public class QuestLogEntryTests
{
    [Test]
    public void Read_WithCompletedFlagSet_ParsesAllFields()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(7);
        writer.WriteString("The Annihilator");
        writer.WriteByte(1);
        var reader = new PacketReader(writer.ToArray());

        var entry = QuestLogEntry.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(entry.QuestId, Is.EqualTo(7));
            Assert.That(entry.Name, Is.EqualTo("The Annihilator"));
            Assert.That(entry.IsCompleted, Is.True);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithCompletedFlagClear_ParsesFalse()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(7);
        writer.WriteString("The Annihilator");
        writer.WriteByte(0);
        var reader = new PacketReader(writer.ToArray());

        var entry = QuestLogEntry.Read(reader);

        Assert.That(entry.IsCompleted, Is.False);
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new QuestLogEntry(0, null!, false));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => QuestLogEntry.Read(null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 0 });

        Assert.Throws<ProtocolException>(() => QuestLogEntry.Read(reader));
    }
}
