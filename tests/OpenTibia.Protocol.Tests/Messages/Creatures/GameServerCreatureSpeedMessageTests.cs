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
public class GameServerCreatureSpeedMessageTests
{
    [Test]
    public void Read_ParsesCreatureIdAndSpeed()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(777u);
        writer.WriteUInt16(220);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreatureSpeedMessage)GameServerCreatureSpeedMessage.Read(GameServerOpcode.CreatureSpeed, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CreatureSpeed));
            Assert.That(message.CreatureId, Is.EqualTo(777u));
            Assert.That(message.Speed, Is.EqualTo(220));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCreatureSpeedMessage.Read(GameServerOpcode.CreatureSpeed, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3 });

        Assert.Throws<ProtocolException>(() => GameServerCreatureSpeedMessage.Read(GameServerOpcode.CreatureSpeed, reader));
    }
}
