// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Effects;

[TestFixture]
public class GameServerAmbientMessageTests
{
    [Test]
    public void Read_ParsesTheLightLevelAndColor()
    {
        var reader = new PacketReader(new byte[] { 5, 215 });

        var message = (GameServerAmbientMessage)GameServerAmbientMessage.Read(GameServerOpcode.Ambient, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.Ambient));
            Assert.That(message.Light.Level, Is.EqualTo(5));
            Assert.That(message.Light.Color, Is.EqualTo(215));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerAmbientMessage.Read(GameServerOpcode.Ambient, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 5 });

        Assert.Throws<ProtocolException>(() => GameServerAmbientMessage.Read(GameServerOpcode.Ambient, reader));
    }
}
