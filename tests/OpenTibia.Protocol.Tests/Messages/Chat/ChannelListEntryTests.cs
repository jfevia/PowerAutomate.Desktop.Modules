// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Chat;

[TestFixture]
public class ChannelListEntryTests
{
    [Test]
    public void Constructor_KeepsFields()
    {
        var entry = new ChannelListEntry(3, "World Chat");

        Assert.Multiple(() =>
        {
            Assert.That(entry.ChannelId, Is.EqualTo(3));
            Assert.That(entry.ChannelName, Is.EqualTo("World Chat"));
        });
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ChannelListEntry(1, null!));
    }

    [Test]
    public void Read_ParsesIdThenName()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(7);
        writer.WriteString("Trade");
        var reader = new PacketReader(writer.ToArray());

        var entry = ChannelListEntry.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(entry.ChannelId, Is.EqualTo(7));
            Assert.That(entry.ChannelName, Is.EqualTo("Trade"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ChannelListEntry.Read(null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0x01, 0x00, 0x05, 0x00, 0x68 });

        Assert.Throws<ProtocolException>(() => ChannelListEntry.Read(reader));
    }
}
