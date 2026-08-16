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
public class GameServerOpenContainerMessageTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithItems_ParsesAllFieldsInOrder()
    {
        var writer = new PacketWriter();
        writer.WriteByte(2);
        writer.WriteUInt16(1987);
        writer.WriteString("Backpack");
        writer.WriteByte(20);
        writer.WriteByte(1);
        writer.WriteByte(2);
        ItemStackCodec.Write(writer, new ItemStack(100, 5), ItemTypes);
        ItemStackCodec.Write(writer, new ItemStack(50, 0), ItemTypes);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerOpenContainerMessage)GameServerOpenContainerMessage.Read(GameServerOpcode.OpenContainer, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.OpenContainer));
            Assert.That(message.ContainerIndex, Is.EqualTo(2));
            Assert.That(message.ContainerItemId, Is.EqualTo(1987));
            Assert.That(message.ContainerName, Is.EqualTo("Backpack"));
            Assert.That(message.Capacity, Is.EqualTo(20));
            Assert.That(message.IsParentAvailable, Is.True);
            Assert.That(message.Items, Has.Count.EqualTo(2));
            Assert.That(message.Items[0].ItemId, Is.EqualTo(100));
            Assert.That(message.Items[0].Extra, Is.EqualTo(5));
            Assert.That(message.Items[1].ItemId, Is.EqualTo(50));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNoParentAndNoItems_DecodesEmptyItemList()
    {
        var writer = new PacketWriter();
        writer.WriteByte(0);
        writer.WriteUInt16(1987);
        writer.WriteString("Backpack");
        writer.WriteByte(20);
        writer.WriteByte(0);
        writer.WriteByte(0);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerOpenContainerMessage)GameServerOpenContainerMessage.Read(GameServerOpcode.OpenContainer, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsParentAvailable, Is.False);
            Assert.That(message.Items, Is.Empty);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullContainerName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new GameServerOpenContainerMessage(0, 0, null!, 0, false, Array.Empty<ItemStack>()));
    }

    [Test]
    public void Constructor_WithNullItems_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new GameServerOpenContainerMessage(0, 0, string.Empty, 0, false, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerOpenContainerMessage.Read(GameServerOpcode.OpenContainer, null!, ItemTypes));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerOpenContainerMessage.Read(GameServerOpcode.OpenContainer, new PacketReader(new byte[6]), null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        writer.WriteByte(2);
        writer.WriteUInt16(1987);
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => GameServerOpenContainerMessage.Read(GameServerOpcode.OpenContainer, reader, ItemTypes));
    }
}
