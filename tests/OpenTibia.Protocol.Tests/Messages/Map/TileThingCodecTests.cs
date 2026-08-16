// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

[TestFixture]
public class TileThingCodecTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void Read_WithKnownCreatureMarker_DecodesCreatureAndNullItem()
    {
        var writer = new PacketWriter();
        CreaturePayloadWriter.WriteKnown(writer, 42);
        var reader = new PacketReader(writer.ToArray());

        var thing = TileThingCodec.Read(reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(thing.Creature!.CreatureId, Is.EqualTo(42u));
            Assert.That(thing.Item, Is.Null);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithItem_DecodesItemAndNullCreature()
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(100);
        writer.WriteByte(5);
        var reader = new PacketReader(writer.ToArray());

        var thing = TileThingCodec.Read(reader, ItemTypes);

        Assert.Multiple(() =>
        {
            Assert.That(thing.Item!.ItemId, Is.EqualTo(100));
            Assert.That(thing.Item!.Extra, Is.EqualTo(5));
            Assert.That(thing.Creature, Is.Null);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => TileThingCodec.Read(null!, ItemTypes));
    }

    [Test]
    public void Read_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => TileThingCodec.Read(new PacketReader(new byte[2]), null!));
    }
}
