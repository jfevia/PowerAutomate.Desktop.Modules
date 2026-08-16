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
public class GameServerTextEffectMessageTests
{
    private static byte[] BuildPayload()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        writer.WriteByte((byte)TextColor.Red);
        writer.WriteString("-25");
        return writer.ToArray();
    }

    [Test]
    public void Read_ParsesPositionColorAndText()
    {
        var reader = new PacketReader(BuildPayload());

        var message = (GameServerTextEffectMessage)GameServerTextEffectMessage.Read(GameServerOpcode.TextEffect, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.TextEffect));
            Assert.That(message.Position, Is.EqualTo(new Position(1, 2, 3)));
            Assert.That(message.Color, Is.EqualTo(TextColor.Red));
            Assert.That(message.Text, Is.EqualTo("-25"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerTextEffectMessage.Read(GameServerOpcode.TextEffect, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payload = BuildPayload();
        var reader = new PacketReader(payload, 0, payload.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerTextEffectMessage.Read(GameServerOpcode.TextEffect, reader));
    }
}
