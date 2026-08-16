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
public class GameServerCreateContainerMessageTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_ParsesContainerIndexThenItem()
    {
        var writer = new PacketWriter();
        writer.WriteByte(1);
        ItemStackCodec.Write(writer, new ItemStack(100, 7), ItemTypes);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreateContainerMessage)GameServerCreateContainerMessage.Read(GameServerOpcode.CreateContainer, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CreateContainer));
            Assert.That(message.ContainerIndex, Is.EqualTo(1));
            Assert.That(message.Item.ItemId, Is.EqualTo(100));
            Assert.That(message.Item.Extra, Is.EqualTo(7));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullItem_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerCreateContainerMessage(0, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCreateContainerMessage.Read(GameServerOpcode.CreateContainer, null!, ItemTypes));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerCreateContainerMessage.Read(GameServerOpcode.CreateContainer, new PacketReader(new byte[4]), null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1 });

        Assert.Throws<ProtocolException>(() => GameServerCreateContainerMessage.Read(GameServerOpcode.CreateContainer, reader, ItemTypes));
    }
}
