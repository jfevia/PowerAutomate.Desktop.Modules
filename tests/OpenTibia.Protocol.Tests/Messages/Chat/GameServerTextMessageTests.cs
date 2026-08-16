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
public class GameServerTextMessageTests
{
    [Test]
    public void Read_ParsesMessageClassAndText()
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)MessageClass.EventAdvance);
        writer.WriteString("You see a sword.");
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerTextMessage)GameServerTextMessage.Read(GameServerOpcode.TextMessage, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.TextMessage));
            Assert.That(message.MessageClass, Is.EqualTo(MessageClass.EventAdvance));
            Assert.That(message.Text, Is.EqualTo("You see a sword."));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullText_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerTextMessage(MessageClass.None, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerTextMessage.Read(GameServerOpcode.TextMessage, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { (byte)MessageClass.None, 0x05, 0x00, 0x68 });

        Assert.Throws<ProtocolException>(() => GameServerTextMessage.Read(GameServerOpcode.TextMessage, reader));
    }
}
