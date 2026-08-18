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
public class GameServerDeleteInventoryMessageTests
{
    [Test]
    public void Read_ParsesSlot()
    {
        var reader = new PacketReader(new byte[] { (byte)InventorySlot.Head });

        var message = (GameServerDeleteInventoryMessage)GameServerDeleteInventoryMessage.Read(GameServerOpcode.DeleteInventory, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.DeleteInventory));
            Assert.That(message.Slot, Is.EqualTo((byte)InventorySlot.Head));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerDeleteInventoryMessage.Read(GameServerOpcode.DeleteInventory, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(Array.Empty<byte>());

        Assert.Throws<ProtocolException>(() => GameServerDeleteInventoryMessage.Read(GameServerOpcode.DeleteInventory, reader));
    }
}
