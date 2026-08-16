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
public class GameServerCloseChannelMessageTests
{
    [Test]
    public void Read_ParsesChannelId()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(9);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCloseChannelMessage)GameServerCloseChannelMessage.Read(GameServerOpcode.CloseChannel, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CloseChannel));
            Assert.That(message.ChannelId, Is.EqualTo(9));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCloseChannelMessage.Read(GameServerOpcode.CloseChannel, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0x01 });

        Assert.Throws<ProtocolException>(() => GameServerCloseChannelMessage.Read(GameServerOpcode.CloseChannel, reader));
    }
}
