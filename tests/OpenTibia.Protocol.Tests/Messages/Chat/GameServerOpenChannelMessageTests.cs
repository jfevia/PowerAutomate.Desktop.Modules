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
public class GameServerOpenChannelMessageTests
{
    [Test]
    public void Read_ParsesChannelIdThenName()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(4);
        writer.WriteString("Trade");
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerOpenChannelMessage)GameServerOpenChannelMessage.Read(GameServerOpcode.OpenChannel, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.OpenChannel));
            Assert.That(message.ChannelId, Is.EqualTo(4));
            Assert.That(message.ChannelName, Is.EqualTo("Trade"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerOpenChannelMessage(1, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerOpenChannelMessage.Read(GameServerOpcode.OpenChannel, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0x01, 0x00, 0x05, 0x00, 0x68 });

        Assert.Throws<ProtocolException>(() => GameServerOpenChannelMessage.Read(GameServerOpcode.OpenChannel, reader));
    }
}
