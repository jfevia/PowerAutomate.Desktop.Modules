// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Trade;

[TestFixture]
public class GameServerPlayerGoodsMessageTests
{
    [Test]
    public void Read_WithGoods_ParsesMoneyThenEveryEntry()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(15000);
        writer.WriteByte(2);
        writer.WriteUInt16(100);
        writer.WriteByte(5);
        writer.WriteUInt16(200);
        writer.WriteByte(10);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerPlayerGoodsMessage)GameServerPlayerGoodsMessage.Read(GameServerOpcode.PlayerGoods, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.PlayerGoods));
            Assert.That(message.Money, Is.EqualTo(15000u));
            Assert.That(message.Goods, Has.Count.EqualTo(2));
            Assert.That(message.Goods[0].ItemId, Is.EqualTo(100));
            Assert.That(message.Goods[1].Count, Is.EqualTo(10));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNoGoods_DecodesEmptyList()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(0);
        writer.WriteByte(0);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerPlayerGoodsMessage)GameServerPlayerGoodsMessage.Read(GameServerOpcode.PlayerGoods, reader);

        Assert.That(message.Goods, Is.Empty);
    }

    [Test]
    public void Constructor_WithNullGoods_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerPlayerGoodsMessage(0, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerPlayerGoodsMessage.Read(GameServerOpcode.PlayerGoods, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 0, 0, 0 });

        Assert.Throws<ProtocolException>(() => GameServerPlayerGoodsMessage.Read(GameServerOpcode.PlayerGoods, reader));
    }
}
