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
public class GameServerCreatureShieldMessageTests
{
    [Test]
    public void Read_ParsesCreatureIdAndShield()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(222u);
        writer.WriteByte(5);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreatureShieldMessage)GameServerCreatureShieldMessage.Read(GameServerOpcode.CreatureParty, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CreatureParty));
            Assert.That(message.CreatureId, Is.EqualTo(222u));
            Assert.That(message.Shield, Is.EqualTo((byte)5));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCreatureShieldMessage.Read(GameServerOpcode.CreatureParty, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3 });

        Assert.Throws<ProtocolException>(() => GameServerCreatureShieldMessage.Read(GameServerOpcode.CreatureParty, reader));
    }
}
