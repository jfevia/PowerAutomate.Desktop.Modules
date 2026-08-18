// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

[TestFixture]
public class GameServerDeleteOnMapMessageTests
{
    [Test]
    public void Read_WithValidPayload_DecodesPositionAndStackPosition()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        writer.WriteByte(4);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerDeleteOnMapMessage)GameServerDeleteOnMapMessage.Read(GameServerOpcode.DeleteOnMap, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.DeleteOnMap));
            Assert.That(message.Position, Is.EqualTo(new Position(1, 2, 3)));
            Assert.That(message.StackPosition, Is.EqualTo(4));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerDeleteOnMapMessage.Read(GameServerOpcode.DeleteOnMap, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => GameServerDeleteOnMapMessage.Read(GameServerOpcode.DeleteOnMap, reader));
    }
}
