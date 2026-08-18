// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Chat;

[TestFixture]
public class GameServerOpenPrivateChannelMessageTests
{
    [Test]
    public void Read_ParsesReceiverName()
    {
        var writer = new PacketWriter();
        writer.WriteString("Al");
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerOpenPrivateChannelMessage)GameServerOpenPrivateChannelMessage.Read(GameServerOpcode.OpenPrivateChannel, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.OpenPrivateChannel));
            Assert.That(message.ReceiverName, Is.EqualTo("Al"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerOpenPrivateChannelMessage(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerOpenPrivateChannelMessage.Read(GameServerOpcode.OpenPrivateChannel, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0x05, 0x00, 0x68 });

        Assert.Throws<ProtocolException>(() => GameServerOpenPrivateChannelMessage.Read(GameServerOpcode.OpenPrivateChannel, reader));
    }
}
