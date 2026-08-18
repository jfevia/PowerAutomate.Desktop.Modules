// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Movement;

[TestFixture]
public class GameServerMoveCreatureMessageTests
{
    [Test]
    public void Read_ParsesFromPositionStackAndToPosition()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(100, 200, 7));
        writer.WriteByte(3);
        PositionCodec.Write(writer, new Position(101, 200, 7));
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerMoveCreatureMessage)GameServerMoveCreatureMessage.Read(GameServerOpcode.MoveCreature, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.MoveCreature));
            Assert.That(message.FromPosition, Is.EqualTo(new Position(100, 200, 7)));
            Assert.That(message.FromStackPosition, Is.EqualTo(3));
            Assert.That(message.ToPosition, Is.EqualTo(new Position(101, 200, 7)));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerMoveCreatureMessage.Read(GameServerOpcode.MoveCreature, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(100, 200, 7));
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => GameServerMoveCreatureMessage.Read(GameServerOpcode.MoveCreature, reader));
    }
}
