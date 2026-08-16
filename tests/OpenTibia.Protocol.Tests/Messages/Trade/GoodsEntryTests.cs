// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Trade;

[TestFixture]
public class GoodsEntryTests
{
    [Test]
    public void Read_ParsesItemIdThenCount()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(2148);
        writer.WriteByte(12);
        var reader = new PacketReader(writer.ToArray());

        var entry = GoodsEntry.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(entry.ItemId, Is.EqualTo(2148));
            Assert.That(entry.Count, Is.EqualTo(12));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GoodsEntry.Read(null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1 });

        Assert.Throws<ProtocolException>(() => GoodsEntry.Read(reader));
    }
}
