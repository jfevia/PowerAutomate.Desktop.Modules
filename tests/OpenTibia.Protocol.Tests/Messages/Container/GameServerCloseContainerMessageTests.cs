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
public class GameServerCloseContainerMessageTests
{
    [Test]
    public void Read_ParsesContainerIndex()
    {
        var reader = new PacketReader(new byte[] { 3 });

        var message = (GameServerCloseContainerMessage)GameServerCloseContainerMessage.Read(GameServerOpcode.CloseContainer, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CloseContainer));
            Assert.That(message.ContainerIndex, Is.EqualTo(3));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCloseContainerMessage.Read(GameServerOpcode.CloseContainer, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(Array.Empty<byte>());

        Assert.Throws<ProtocolException>(() => GameServerCloseContainerMessage.Read(GameServerOpcode.CloseContainer, reader));
    }
}
