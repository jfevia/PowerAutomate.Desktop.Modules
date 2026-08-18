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
public class GameServerCreatureSkullMessageTests
{
    [Test]
    public void Read_ParsesCreatureIdAndSkull()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(444u);
        writer.WriteByte(3);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreatureSkullMessage)GameServerCreatureSkullMessage.Read(GameServerOpcode.CreatureSkull, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CreatureSkull));
            Assert.That(message.CreatureId, Is.EqualTo(444u));
            Assert.That(message.Skull, Is.EqualTo((byte)3));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCreatureSkullMessage.Read(GameServerOpcode.CreatureSkull, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3 });

        Assert.Throws<ProtocolException>(() => GameServerCreatureSkullMessage.Read(GameServerOpcode.CreatureSkull, reader));
    }
}
