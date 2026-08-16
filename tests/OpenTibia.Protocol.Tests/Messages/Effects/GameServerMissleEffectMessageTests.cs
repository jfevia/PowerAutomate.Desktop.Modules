// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Effects;

[TestFixture]
public class GameServerMissleEffectMessageTests
{
    private static byte[] BuildPayload(byte wireEffectByte)
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(10, 20, 7));
        PositionCodec.Write(writer, new Position(15, 25, 7));
        writer.WriteByte(wireEffectByte);
        return writer.ToArray();
    }

    [TestCase((byte)1, ShootEffect.Spear)]
    [TestCase((byte)42, ShootEffect.Cake)]
    [TestCase((byte)255, ShootEffect.Weapontype)]
    [TestCase((byte)0, ShootEffect.None)]
    public void Read_DecodesTheWirePlusOneEffectByte(byte wireEffectByte, ShootEffect expected)
    {
        var reader = new PacketReader(BuildPayload(wireEffectByte));

        var message = (GameServerMissleEffectMessage)GameServerMissleEffectMessage.Read(GameServerOpcode.MissleEffect, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.MissleEffect));
            Assert.That(message.FromPosition, Is.EqualTo(new Position(10, 20, 7)));
            Assert.That(message.ToPosition, Is.EqualTo(new Position(15, 25, 7)));
            Assert.That(message.EffectType, Is.EqualTo(expected));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerMissleEffectMessage.Read(GameServerOpcode.MissleEffect, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payload = BuildPayload(1);
        var reader = new PacketReader(payload, 0, payload.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerMissleEffectMessage.Read(GameServerOpcode.MissleEffect, reader));
    }
}
