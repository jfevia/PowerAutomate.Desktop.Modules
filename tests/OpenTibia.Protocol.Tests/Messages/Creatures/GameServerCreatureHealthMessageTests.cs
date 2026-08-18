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
public class GameServerCreatureHealthMessageTests
{
    [Test]
    public void Read_ParsesCreatureIdAndHealthPercent()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(555u);
        writer.WriteByte(72);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreatureHealthMessage)GameServerCreatureHealthMessage.Read(GameServerOpcode.CreatureHealth, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CreatureHealth));
            Assert.That(message.CreatureId, Is.EqualTo(555u));
            Assert.That(message.HealthPercent, Is.EqualTo(72));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCreatureHealthMessage.Read(GameServerOpcode.CreatureHealth, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3 });

        Assert.Throws<ProtocolException>(() => GameServerCreatureHealthMessage.Read(GameServerOpcode.CreatureHealth, reader));
    }
}
