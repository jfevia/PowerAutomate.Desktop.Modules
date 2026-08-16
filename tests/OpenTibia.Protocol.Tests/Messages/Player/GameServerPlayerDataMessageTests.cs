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
public class GameServerPlayerDataMessageTests
{
    private static byte[] BuildPayload()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(120);
        writer.WriteUInt16(150);
        writer.WriteUInt32(543210u);
        writer.WriteUInt32(9876543u);
        writer.WriteUInt16(80);
        writer.WriteByte(45);
        writer.WriteUInt16(300);
        writer.WriteUInt16(350);
        writer.WriteByte(12);
        writer.WriteByte(66);
        writer.WriteByte(100);
        writer.WriteUInt16(2400);
        return writer.ToArray();
    }

    [Test]
    public void Read_ParsesEveryFieldInWireOrder()
    {
        var reader = new PacketReader(BuildPayload());

        var message = (GameServerPlayerDataMessage)GameServerPlayerDataMessage.Read(GameServerOpcode.PlayerData, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.PlayerData));
            Assert.That(message.Health, Is.EqualTo(120));
            Assert.That(message.MaxHealth, Is.EqualTo(150));
            Assert.That(message.CapacityHundredths, Is.EqualTo(543210u));
            Assert.That(message.Experience, Is.EqualTo(9876543u));
            Assert.That(message.Level, Is.EqualTo(80));
            Assert.That(message.LevelPercent, Is.EqualTo(45));
            Assert.That(message.Mana, Is.EqualTo(300));
            Assert.That(message.MaxMana, Is.EqualTo(350));
            Assert.That(message.MagicLevel, Is.EqualTo(12));
            Assert.That(message.MagicLevelPercent, Is.EqualTo(66));
            Assert.That(message.Soul, Is.EqualTo(100));
            Assert.That(message.StaminaMinutes, Is.EqualTo(2400));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerPlayerDataMessage.Read(GameServerOpcode.PlayerData, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payload = BuildPayload();
        var reader = new PacketReader(payload, 0, payload.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerPlayerDataMessage.Read(GameServerOpcode.PlayerData, reader));
    }
}
