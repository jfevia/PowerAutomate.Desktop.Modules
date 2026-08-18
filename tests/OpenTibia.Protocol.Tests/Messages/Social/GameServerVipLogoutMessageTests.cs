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
public class GameServerVipLogoutMessageTests
{
    [Test]
    public void Read_ParsesIdOnly()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(654u);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerVipLogoutMessage)GameServerVipLogoutMessage.Read(GameServerOpcode.VipLogout, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.VipLogout));
            Assert.That(message.Id, Is.EqualTo(654u));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerVipLogoutMessage.Read(GameServerOpcode.VipLogout, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0x01, 0x00, 0x00 });

        Assert.Throws<ProtocolException>(() => GameServerVipLogoutMessage.Read(GameServerOpcode.VipLogout, reader));
    }
}
