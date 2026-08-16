// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Container;

[TestFixture]
public class GameServerDeleteInContainerMessageTests
{
    [Test]
    public void Read_ParsesContainerIndexThenSlot()
    {
        var reader = new PacketReader(new byte[] { 1, 4 });

        var message = (GameServerDeleteInContainerMessage)GameServerDeleteInContainerMessage.Read(GameServerOpcode.DeleteInContainer, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.DeleteInContainer));
            Assert.That(message.ContainerIndex, Is.EqualTo(1));
            Assert.That(message.Slot, Is.EqualTo(4));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerDeleteInContainerMessage.Read(GameServerOpcode.DeleteInContainer, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1 });

        Assert.Throws<ProtocolException>(() => GameServerDeleteInContainerMessage.Read(GameServerOpcode.DeleteInContainer, reader));
    }
}
