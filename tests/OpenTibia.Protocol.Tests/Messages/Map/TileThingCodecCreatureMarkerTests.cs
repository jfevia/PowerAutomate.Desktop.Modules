// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

/// <summary>
/// Regression cover for a live desync: item ids sharing a creature marker low byte.
/// </summary>
[TestFixture]
public class TileThingCodecCreatureMarkerTests
{
    private sealed class NoItemTypes : Protocol.Items.IItemTypeProvider
    {
        public bool IsStackable(ushort itemId) => false;

        public bool IsFluidContainer(ushort itemId) => false;

        public bool IsSplash(ushort itemId) => false;
    }

    [TestCase((ushort)0x0061)]
    [TestCase((ushort)0x0062)]
    public void HasCreatureMarker_WithRealCreatureMarker_IsTrue(ushort marker)
    {
        var reader = new PacketReader(new[] { (byte)(marker & 0xFF), (byte)(marker >> 8) });

        Assert.That(TileThingCodec.HasCreatureMarker(reader), Is.True);
    }

    [TestCase((ushort)0x0E61)]
    [TestCase((ushort)0x0E62)]
    [TestCase((ushort)0x1161)]
    [TestCase((ushort)0xFF62)]
    public void HasCreatureMarker_WithItemIdSharingTheLowByte_IsFalse(ushort itemId)
    {
        // 0x0E61 is a real town item; peeking only the low byte misread it as a creature and desynced the map.
        var reader = new PacketReader(new[] { (byte)(itemId & 0xFF), (byte)(itemId >> 8) });

        Assert.That(TileThingCodec.HasCreatureMarker(reader), Is.False);
    }

    [Test]
    public void HasCreatureMarker_WithASingleRemainingByte_IsFalse()
    {
        var reader = new PacketReader(new byte[] { 0x61 });

        Assert.That(TileThingCodec.HasCreatureMarker(reader), Is.False);
    }

    [Test]
    public void HasCreatureMarker_WithNullReader_Throws()
    {
        Assert.Throws<System.ArgumentNullException>(() => TileThingCodec.HasCreatureMarker(null!));
    }

    [Test]
    public void Read_WithItemIdSharingTheLowByte_ReturnsAnItemNotACreature()
    {
        var reader = new PacketReader(new byte[] { 0x61, 0x0E });

        var thing = TileThingCodec.Read(reader, new NoItemTypes());

        Assert.Multiple(() =>
        {
            Assert.That(thing.Creature, Is.Null);
            Assert.That(thing.Item, Is.Not.Null);
            Assert.That(thing.Item!.ItemId, Is.EqualTo(0x0E61));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }
}

[TestFixture]
public class PacketReaderPeekUInt16Tests
{
    [Test]
    public void PeekUInt16_ReadsLittleEndianWithoutConsuming()
    {
        var reader = new PacketReader(new byte[] { 0x61, 0x0E, 0xAA });

        Assert.Multiple(() =>
        {
            Assert.That(reader.PeekUInt16(), Is.EqualTo(0x0E61));
            Assert.That(reader.Position, Is.Zero);
            Assert.That(reader.ReadUInt16(), Is.EqualTo(0x0E61));
        });
    }

    [Test]
    public void PeekUInt16_WithoutTwoBytes_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0x61 });

        Assert.Throws<ProtocolException>(() => reader.PeekUInt16());
    }
}
