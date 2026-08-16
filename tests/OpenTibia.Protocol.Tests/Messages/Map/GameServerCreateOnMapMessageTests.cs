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
public class GameServerCreateOnMapMessageTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithItem_DecodesPositionStackPositionAndItemThing()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        writer.WriteByte(4);
        writer.WriteUInt16(100);
        writer.WriteByte(9);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreateOnMapMessage)GameServerCreateOnMapMessage.Read(GameServerOpcode.CreateOnMap, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CreateOnMap));
            Assert.That(message.Position, Is.EqualTo(new Position(1, 2, 3)));
            Assert.That(message.StackPosition, Is.EqualTo(4));
            Assert.That(message.Thing.Item!.ItemId, Is.EqualTo(100));
            Assert.That(message.Thing.Creature, Is.Null);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithCreature_DecodesCreatureThing()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        writer.WriteByte(4);
        CreaturePayloadWriter.WriteKnown(writer, 42);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreateOnMapMessage)GameServerCreateOnMapMessage.Read(GameServerOpcode.CreateOnMap, reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(message.Thing.Creature!.CreatureId, Is.EqualTo(42u));
            Assert.That(message.Thing.Item, Is.Null);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCreateOnMapMessage.Read(GameServerOpcode.CreateOnMap, null!, ItemTypes));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            GameServerCreateOnMapMessage.Read(GameServerOpcode.CreateOnMap, new PacketReader(new byte[6]), null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        PositionCodec.Write(writer, new Position(1, 2, 3));
        writer.WriteByte(4);
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => GameServerCreateOnMapMessage.Read(GameServerOpcode.CreateOnMap, reader, ItemTypes));
    }
}
