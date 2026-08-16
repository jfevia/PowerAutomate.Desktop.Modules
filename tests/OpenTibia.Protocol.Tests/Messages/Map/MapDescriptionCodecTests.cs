// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

[TestFixture]
public class MapDescriptionCodecTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    private static void WriteTerminator(PacketWriter writer, int skip)
    {
        writer.WriteUInt16((ushort)(0xFF00 | skip));
    }

    [Test]
    public void ReadRawFloors_WithItemsCreaturesAndSkipRuns_DecodesEveryTile()
    {
        var writer = new PacketWriter();

        // Floor 0, tile 0: stackable item.
        writer.WriteUInt16(100);
        writer.WriteByte(42);
        WriteTerminator(writer, 0);

        // Floor 0, tile 1: known creature.
        CreaturePayloadWriter.WriteKnown(writer, 555);
        WriteTerminator(writer, 0);

        // Floor 0, tile 2: unknown creature, then a fluid-container item.
        CreaturePayloadWriter.WriteUnknown(writer, 777, "Rat");
        writer.WriteUInt16(200);
        writer.WriteByte(3);
        WriteTerminator(writer, 0);

        // Floor 0, tile 3: plain item, then a skip run crossing into floor 1.
        writer.WriteUInt16(10);
        WriteTerminator(writer, 1);

        // Floor 1, tile 0: skipped (carried over from floor 0's terminator).

        // Floor 1, tile 1: splash item, then a skip run.
        writer.WriteUInt16(300);
        writer.WriteByte(9);
        WriteTerminator(writer, 1);

        // Floor 1, tile 2: skipped.

        // Floor 1, tile 3: empty tile (terminator with no things at all).
        WriteTerminator(writer, 0);

        var reader = new PacketReader(writer.ToArray());

        var floors = MapDescriptionCodec.ReadRawFloors(reader, ItemTypes, 2, 4, 1);

        Assert.Multiple(() =>
        {
            Assert.That(floors, Has.Count.EqualTo(2));
            Assert.That(floors[0], Has.Count.EqualTo(4));
            Assert.That(floors[1], Has.Count.EqualTo(4));

            Assert.That(floors[0][0]!.Things, Has.Count.EqualTo(1));
            Assert.That(floors[0][0]!.Things[0].Item!.ItemId, Is.EqualTo(100));
            Assert.That(floors[0][0]!.Things[0].Item!.Extra, Is.EqualTo(42));
            Assert.That(floors[0][0]!.Things[0].Creature, Is.Null);

            Assert.That(floors[0][1]!.Things, Has.Count.EqualTo(1));
            Assert.That(floors[0][1]!.Things[0].Creature!.CreatureId, Is.EqualTo(555u));
            Assert.That(floors[0][1]!.Things[0].Creature!.IsKnown, Is.True);
            Assert.That(floors[0][1]!.Things[0].Item, Is.Null);

            Assert.That(floors[0][2]!.Things, Has.Count.EqualTo(2));
            Assert.That(floors[0][2]!.Things[0].Creature!.CreatureId, Is.EqualTo(777u));
            Assert.That(floors[0][2]!.Things[0].Creature!.IsKnown, Is.False);
            Assert.That(floors[0][2]!.Things[0].Creature!.Name, Is.EqualTo("Rat"));
            Assert.That(floors[0][2]!.Things[1].Item!.ItemId, Is.EqualTo(200));
            Assert.That(floors[0][2]!.Things[1].Item!.Extra, Is.EqualTo(3));

            Assert.That(floors[0][3]!.Things, Has.Count.EqualTo(1));
            Assert.That(floors[0][3]!.Things[0].Item!.ItemId, Is.EqualTo(10));
            Assert.That(floors[0][3]!.Things[0].Item!.Extra, Is.Zero);

            Assert.That(floors[1][0], Is.Null);

            Assert.That(floors[1][1]!.Things, Has.Count.EqualTo(1));
            Assert.That(floors[1][1]!.Things[0].Item!.ItemId, Is.EqualTo(300));
            Assert.That(floors[1][1]!.Things[0].Item!.Extra, Is.EqualTo(9));

            Assert.That(floors[1][2], Is.Null);

            Assert.That(floors[1][3]!.Things, Is.Empty);

            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithUndergroundCenterZ_DecodesFiveNearbyFloors()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(100);
        writer.WriteByte(1);
        WriteTerminator(writer, 4);
        var reader = new PacketReader(writer.ToArray());

        var floors = MapDescriptionCodec.Read(reader, ItemTypes, 10, 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(floors, Has.Count.EqualTo(5));
            Assert.That(floors[0][0]!.Things[0].Item!.ItemId, Is.EqualTo(100));
            for (var i = 1; i < 5; i++)
            {
                Assert.That(floors[i][0], Is.Null);
            }

            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithUndergroundCenterZAtMax_ClampsFloorRange()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(100);
        writer.WriteByte(1);
        WriteTerminator(writer, 2);
        var reader = new PacketReader(writer.ToArray());

        var floors = MapDescriptionCodec.Read(reader, ItemTypes, 15, 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(floors, Has.Count.EqualTo(3));
            Assert.That(floors[0][0]!.Things[0].Item!.ItemId, Is.EqualTo(100));
            Assert.That(floors[1][0], Is.Null);
            Assert.That(floors[2][0], Is.Null);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithSurfaceCenterZ_DecodesEightFloorsDownToGroundLevel()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(100);
        writer.WriteByte(1);
        WriteTerminator(writer, 7);
        var reader = new PacketReader(writer.ToArray());

        var floors = MapDescriptionCodec.Read(reader, ItemTypes, 0, 1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(floors, Has.Count.EqualTo(8));
            Assert.That(floors[0][0]!.Things[0].Item!.ItemId, Is.EqualTo(100));
            for (var i = 1; i < 8; i++)
            {
                Assert.That(floors[i][0], Is.Null);
            }

            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(100);
        var reader = new PacketReader(writer.ToArray());

        Assert.Throws<ProtocolException>(() => MapDescriptionCodec.Read(reader, ItemTypes, 0, 1, 1));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MapDescriptionCodec.Read(null!, ItemTypes, 0, 1, 1));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MapDescriptionCodec.Read(new PacketReader(new byte[2]), null!, 0, 1, 1));
    }

    [Test]
    public void ReadRawFloors_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MapDescriptionCodec.ReadRawFloors(null!, ItemTypes, 1, 1, 1));
    }

    [Test]
    public void ReadRawFloors_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MapDescriptionCodec.ReadRawFloors(new PacketReader(new byte[2]), null!, 1, 1, 1));
    }
}
