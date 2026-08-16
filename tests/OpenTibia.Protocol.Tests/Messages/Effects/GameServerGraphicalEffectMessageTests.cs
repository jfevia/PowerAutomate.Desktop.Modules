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
public class GameServerGraphicalEffectMessageTests
{
    private static byte[] BuildPayload(byte wireEffectByte)
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(100, 200, 7));
        writer.WriteByte(wireEffectByte);
        return writer.ToArray();
    }

    [TestCase((byte)1, MagicEffect.DrawBlood)]
    [TestCase((byte)70, MagicEffect.Dragonhead)]
    [TestCase((byte)0, MagicEffect.None)]
    public void Read_DecodesTheWirePlusOneEffectByte(byte wireEffectByte, MagicEffect expected)
    {
        var reader = new PacketReader(BuildPayload(wireEffectByte));

        var message = (GameServerGraphicalEffectMessage)GameServerGraphicalEffectMessage.Read(GameServerOpcode.GraphicalEffect, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.GraphicalEffect));
            Assert.That(message.Position, Is.EqualTo(new Position(100, 200, 7)));
            Assert.That(message.EffectType, Is.EqualTo(expected));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerGraphicalEffectMessage.Read(GameServerOpcode.GraphicalEffect, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payload = BuildPayload(1);
        var reader = new PacketReader(payload, 0, payload.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerGraphicalEffectMessage.Read(GameServerOpcode.GraphicalEffect, reader));
    }
}
