// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

[TestFixture]
public class GameServerChangeOnMapMessageTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithChangedItem_DecodesItemAndNullTurnFields()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        writer.WriteByte(4);
        writer.WriteUInt16(100);
        writer.WriteByte(9);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerChangeOnMapMessage)GameServerChangeOnMapMessage.Read(GameServerOpcode.ChangeOnMap, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.ChangeOnMap));
            Assert.That(message.Position, Is.EqualTo(new Position(1, 2, 3)));
            Assert.That(message.StackPosition, Is.EqualTo(4));
            Assert.That(message.ChangedItem!.ItemId, Is.EqualTo(100));
            Assert.That(message.TurnedCreatureId, Is.Null);
            Assert.That(message.NewDirection, Is.Null);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithTurnMarker_DecodesTurnedCreatureAndNullItem()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        writer.WriteByte(4);
        writer.WriteUInt16(0x0063);
        writer.WriteUInt32(77);
        writer.WriteByte((byte)Direction.West);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerChangeOnMapMessage)GameServerChangeOnMapMessage.Read(GameServerOpcode.ChangeOnMap, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.TurnedCreatureId, Is.EqualTo(77u));
            Assert.That(message.NewDirection, Is.EqualTo(Direction.West));
            Assert.That(message.ChangedItem, Is.Null);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerChangeOnMapMessage.Read(GameServerOpcode.ChangeOnMap, null!, ItemTypes));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerChangeOnMapMessage.Read(GameServerOpcode.ChangeOnMap, new PacketReader(new byte[6]), null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        writer.WriteByte(4);
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => GameServerChangeOnMapMessage.Read(GameServerOpcode.ChangeOnMap, reader, ItemTypes));
    }
}
