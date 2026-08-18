// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Social;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Social;

[TestFixture]
public class GameServerVipAddMessageTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void Read_ParsesIdNameAndOnlineState(bool isOnline)
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(555u);
        writer.WriteString("Friend");
        writer.WriteByte((byte)(isOnline ? 1 : 0));
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerVipAddMessage)GameServerVipAddMessage.Read(GameServerOpcode.VipAdd, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.VipAdd));
            Assert.That(message.Id, Is.EqualTo(555u));
            Assert.That(message.Name, Is.EqualTo("Friend"));
            Assert.That(message.IsOnline, Is.EqualTo(isOnline));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerVipAddMessage(1, null!, false));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerVipAddMessage.Read(GameServerOpcode.VipAdd, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x05, 0x00, 0x68 });

        Assert.Throws<ProtocolException>(() => GameServerVipAddMessage.Read(GameServerOpcode.VipAdd, reader));
    }
}
