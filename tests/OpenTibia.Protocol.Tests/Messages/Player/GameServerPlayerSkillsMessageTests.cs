// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Player;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Player;

[TestFixture]
public class GameServerPlayerSkillsMessageTests
{
    private static byte[] BuildPayload()
    {
        return new byte[]
        {
            10, 11, // fist
            20, 21, // club
            30, 31, // sword
            40, 41, // axe
            50, 51, // distance
            60, 61, // shielding
            70, 71 // fishing
        };
    }

    [Test]
    public void Read_ParsesEverySkillInWireOrder()
    {
        var reader = new PacketReader(BuildPayload());

        var message = (GameServerPlayerSkillsMessage)GameServerPlayerSkillsMessage.Read(GameServerOpcode.PlayerSkills, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.PlayerSkills));
            Assert.That(message.Fist.Level, Is.EqualTo(10));
            Assert.That(message.Fist.Percent, Is.EqualTo(11));
            Assert.That(message.Club.Level, Is.EqualTo(20));
            Assert.That(message.Sword.Level, Is.EqualTo(30));
            Assert.That(message.Axe.Level, Is.EqualTo(40));
            Assert.That(message.Distance.Level, Is.EqualTo(50));
            Assert.That(message.Shielding.Level, Is.EqualTo(60));
            Assert.That(message.Fishing.Level, Is.EqualTo(70));
            Assert.That(message.Fishing.Percent, Is.EqualTo(71));
            Assert.That(message.Skills, Has.Length.EqualTo(7));
            Assert.That(message.Skills[0], Is.SameAs(message.Fist));
            Assert.That(message.Skills[6], Is.SameAs(message.Fishing));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerPlayerSkillsMessage.Read(GameServerOpcode.PlayerSkills, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payload = BuildPayload();
        var reader = new PacketReader(payload, 0, payload.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerPlayerSkillsMessage.Read(GameServerOpcode.PlayerSkills, reader));
    }
}
