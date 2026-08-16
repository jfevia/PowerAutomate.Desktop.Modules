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
public class GameServerOpenOwnChannelMessageTests
{
    [Test]
    public void Read_ParsesChannelIdThenName()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(9);
        writer.WriteString("Party");
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerOpenOwnChannelMessage)GameServerOpenOwnChannelMessage.Read(GameServerOpcode.OpenOwnChannel, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.OpenOwnChannel));
            Assert.That(message.ChannelId, Is.EqualTo(9));
            Assert.That(message.ChannelName, Is.EqualTo("Party"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerOpenOwnChannelMessage(1, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerOpenOwnChannelMessage.Read(GameServerOpcode.OpenOwnChannel, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0x01, 0x00, 0x05, 0x00, 0x68 });

        Assert.Throws<ProtocolException>(() => GameServerOpenOwnChannelMessage.Read(GameServerOpcode.OpenOwnChannel, reader));
    }
}
