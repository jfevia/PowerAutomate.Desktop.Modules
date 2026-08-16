// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Creatures;

[TestFixture]
public class GameServerCreatureSquareMessageTests
{
    [Test]
    public void Read_ParsesCreatureIdAndColor()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(111u);
        writer.WriteByte((byte)TextColor.Red);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreatureSquareMessage)GameServerCreatureSquareMessage.Read(GameServerOpcode.CreatureSquare, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CreatureSquare));
            Assert.That(message.CreatureId, Is.EqualTo(111u));
            Assert.That(message.Color, Is.EqualTo(TextColor.Red));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCreatureSquareMessage.Read(GameServerOpcode.CreatureSquare, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3 });

        Assert.Throws<ProtocolException>(() => GameServerCreatureSquareMessage.Read(GameServerOpcode.CreatureSquare, reader));
    }
}
