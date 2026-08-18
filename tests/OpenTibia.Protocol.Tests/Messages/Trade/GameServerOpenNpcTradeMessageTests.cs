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
public class GameServerOpenNpcTradeMessageTests
{
    [Test]
    public void Read_WithItems_ParsesEveryShopItem()
    {
        var writer = new PacketWriter();
        writer.WriteByte(2);
        WriteShopItem(writer, 100, "Sword");
        WriteShopItem(writer, 200, "Shield");
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerOpenNpcTradeMessage)GameServerOpenNpcTradeMessage.Read(GameServerOpcode.OpenNpcTrade, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.OpenNpcTrade));
            Assert.That(message.Items, Has.Count.EqualTo(2));
            Assert.That(message.Items[0].Name, Is.EqualTo("Sword"));
            Assert.That(message.Items[1].Name, Is.EqualTo("Shield"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNoItems_DecodesEmptyList()
    {
        var reader = new PacketReader(new byte[] { 0 });

        var message = (GameServerOpenNpcTradeMessage)GameServerOpenNpcTradeMessage.Read(GameServerOpcode.OpenNpcTrade, reader);

        Assert.That(message.Items, Is.Empty);
    }

    [Test]
    public void Constructor_WithNullItems_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerOpenNpcTradeMessage(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerOpenNpcTradeMessage.Read(GameServerOpcode.OpenNpcTrade, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1 });

        Assert.Throws<ProtocolException>(() => GameServerOpenNpcTradeMessage.Read(GameServerOpcode.OpenNpcTrade, reader));
    }

    private static void WriteShopItem(PacketWriter writer, ushort clientId, string name)
    {
        writer.WriteUInt16(clientId);
        writer.WriteByte(0);
        writer.WriteString(name);
        writer.WriteUInt32(0);
        writer.WriteUInt32(0);
        writer.WriteUInt32(0);
    }
}
