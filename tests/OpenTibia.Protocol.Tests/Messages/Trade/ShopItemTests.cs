// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Trade;

[TestFixture]
public class ShopItemTests
{
    [Test]
    public void Read_ParsesAllFieldsInOrder()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(2148);
        writer.WriteByte(0);
        writer.WriteString("Fire Sword");
        writer.WriteUInt32(4200);
        writer.WriteUInt32(500);
        writer.WriteUInt32(100);
        var reader = new PacketReader(writer.ToArray());

        var item = ShopItem.Read(reader);

        Assert.Multiple(() =>
        {
            Assert.That(item.ClientId, Is.EqualTo(2148));
            Assert.That(item.SubType, Is.EqualTo(0));
            Assert.That(item.Name, Is.EqualTo("Fire Sword"));
            Assert.That(item.WeightHundredths, Is.EqualTo(4200u));
            Assert.That(item.BuyPrice, Is.EqualTo(500u));
            Assert.That(item.SellPrice, Is.EqualTo(100u));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ShopItem(1, 0, null!, 0, 0, 0));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ShopItem.Read(null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 0 });

        Assert.Throws<ProtocolException>(() => ShopItem.Read(reader));
    }
}
