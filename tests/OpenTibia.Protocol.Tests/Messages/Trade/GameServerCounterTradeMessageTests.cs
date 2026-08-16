// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Trade;

[TestFixture]
public class GameServerCounterTradeMessageTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithItems_ParsesPlayerNameThenEveryItem()
    {
        var writer = new PacketWriter();
        writer.WriteString("Counterparty");
        writer.WriteByte(2);
        ItemStackCodec.Write(writer, new ItemStack(100, 3), ItemTypes);
        ItemStackCodec.Write(writer, new ItemStack(50, 0), ItemTypes);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCounterTradeMessage)GameServerCounterTradeMessage.Read(GameServerOpcode.CounterTrade, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CounterTrade));
            Assert.That(message.PlayerName, Is.EqualTo("Counterparty"));
            Assert.That(message.Items, Has.Count.EqualTo(2));
            Assert.That(message.Items[0].ItemId, Is.EqualTo(100));
            Assert.That(message.Items[0].Extra, Is.EqualTo(3));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNoItems_DecodesEmptyList()
    {
        var writer = new PacketWriter();
        writer.WriteString("Counterparty");
        writer.WriteByte(0);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCounterTradeMessage)GameServerCounterTradeMessage.Read(GameServerOpcode.CounterTrade, reader, ItemTypes);

        Assert.That(message.Items, Is.Empty);
    }

    [Test]
    public void Constructor_WithNullPlayerName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerCounterTradeMessage(null!, Array.Empty<ItemStack>()));
    }

    [Test]
    public void Constructor_WithNullItems_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerCounterTradeMessage(string.Empty, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCounterTradeMessage.Read(GameServerOpcode.CounterTrade, null!, ItemTypes));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerCounterTradeMessage.Read(GameServerOpcode.CounterTrade, new PacketReader(new byte[3]), null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        writer.WriteString("Counterparty");
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => GameServerCounterTradeMessage.Read(GameServerOpcode.CounterTrade, reader, ItemTypes));
    }
}
