// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Container;

[TestFixture]
public class GameServerSetInventoryMessageTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_ParsesSlotThenItem()
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)InventorySlot.Right);
        ItemStackCodec.Write(writer, new ItemStack(2920, 0), ItemTypes);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerSetInventoryMessage)GameServerSetInventoryMessage.Read(GameServerOpcode.SetInventory, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.SetInventory));
            Assert.That(message.Slot, Is.EqualTo((byte)InventorySlot.Right));
            Assert.That(message.Item.ItemId, Is.EqualTo(2920));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullItem_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerSetInventoryMessage(0, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerSetInventoryMessage.Read(GameServerOpcode.SetInventory, null!, ItemTypes));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerSetInventoryMessage.Read(GameServerOpcode.SetInventory, new PacketReader(new byte[3]), null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 5 });

        Assert.Throws<ProtocolException>(() => GameServerSetInventoryMessage.Read(GameServerOpcode.SetInventory, reader, ItemTypes));
    }
}
