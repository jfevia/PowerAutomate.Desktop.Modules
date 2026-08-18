// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Creatures;

[TestFixture]
public class GameServerCreatureLightMessageTests
{
    [Test]
    public void Read_ParsesCreatureIdAndLight()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(888u);
        writer.WriteByte(7);
        writer.WriteByte(215);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreatureLightMessage)GameServerCreatureLightMessage.Read(GameServerOpcode.CreatureLight, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CreatureLight));
            Assert.That(message.CreatureId, Is.EqualTo(888u));
            Assert.That(message.Light.Level, Is.EqualTo(7));
            Assert.That(message.Light.Color, Is.EqualTo(215));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCreatureLightMessage.Read(GameServerOpcode.CreatureLight, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3, 4, 5 });

        Assert.Throws<ProtocolException>(() => GameServerCreatureLightMessage.Read(GameServerOpcode.CreatureLight, reader));
    }
}
