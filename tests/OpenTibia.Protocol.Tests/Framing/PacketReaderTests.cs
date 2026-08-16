// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Framing;

[TestFixture]
public class PacketReaderTests
{
    [Test]
    public void ReadByte_ReturnsBytesInOrder()
    {
        var reader = new PacketReader(new byte[] { 1, 2 });

        Assert.Multiple(() =>
        {
            Assert.That(reader.ReadByte(), Is.EqualTo(1));
            Assert.That(reader.ReadByte(), Is.EqualTo(2));
            Assert.That(reader.Remaining, Is.Zero);
            Assert.That(reader.Position, Is.EqualTo(2));
        });
    }

    [Test]
    public void PeekByte_DoesNotAdvance()
    {
        var reader = new PacketReader(new byte[] { 7 });

        Assert.Multiple(() =>
        {
            Assert.That(reader.PeekByte(), Is.EqualTo(7));
            Assert.That(reader.Position, Is.Zero);
        });
    }

    [Test]
    public void ReadUInt16_ReadsLittleEndian()
    {
        var reader = new PacketReader(new byte[] { 0x34, 0x12 });

        Assert.That(reader.ReadUInt16(), Is.EqualTo(0x1234));
    }

    [Test]
    public void ReadUInt32_ReadsLittleEndian()
    {
        var reader = new PacketReader(new byte[] { 0x78, 0x56, 0x34, 0x12 });

        Assert.That(reader.ReadUInt32(), Is.EqualTo(0x12345678u));
    }

    [Test]
    public void ReadUInt64_ReadsLittleEndian()
    {
        var reader = new PacketReader(new byte[] { 0xEF, 0xCD, 0xAB, 0x89, 0x67, 0x45, 0x23, 0x01 });

        Assert.That(reader.ReadUInt64(), Is.EqualTo(0x0123456789ABCDEFul));
    }

    [Test]
    public void ReadBytes_ReturnsRequestedRange()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3 });

        Assert.That(reader.ReadBytes(2), Is.EqualTo(new byte[] { 1, 2 }));
    }

    [Test]
    public void ReadString_ReadsLengthPrefixedLatin1()
    {
        var reader = new PacketReader(new byte[] { 0x02, 0x00, 0x68, 0x69 });

        Assert.That(reader.ReadString(), Is.EqualTo("hi"));
    }

    [Test]
    public void Skip_AdvancesPosition()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3 });

        reader.Skip(2);

        Assert.That(reader.ReadByte(), Is.EqualTo(3));
    }

    [Test]
    public void Constructor_WithRange_LimitsVisibleWindow()
    {
        var reader = new PacketReader(new byte[] { 9, 1, 2, 9 }, 1, 2);

        Assert.Multiple(() =>
        {
            Assert.That(reader.Remaining, Is.EqualTo(2));
            Assert.That(reader.ReadByte(), Is.EqualTo(1));
        });
    }

    [Test]
    public void Constructor_WithNullBuffer_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new PacketReader(null!));
    }

    [Test]
    public void Constructor_WithNullBufferAndRange_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new PacketReader(null!, 0, 0));
    }

    [Test]
    public void Constructor_WithNegativeOffset_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PacketReader(new byte[1], -1, 0));
    }

    [Test]
    public void Constructor_WithNegativeCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PacketReader(new byte[1], 0, -1));
    }

    [Test]
    public void Constructor_WithRangeBeyondBuffer_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PacketReader(new byte[1], 1, 1));
    }

    [Test]
    public void ReadByte_PastEnd_ThrowsProtocolException()
    {
        var reader = new PacketReader(Array.Empty<byte>());

        Assert.Throws<ProtocolException>(() => reader.ReadByte());
    }

    [Test]
    public void ReadBytes_WithNegativeCount_Throws()
    {
        var reader = new PacketReader(new byte[1]);

        Assert.Throws<ArgumentOutOfRangeException>(() => reader.ReadBytes(-1));
    }

    [Test]
    public void ReadString_WithTruncatedBody_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0x05, 0x00, 0x68 });

        Assert.Throws<ProtocolException>(() => reader.ReadString());
    }

    [Test]
    public void Skip_WithNegativeCount_Throws()
    {
        var reader = new PacketReader(new byte[1]);

        Assert.Throws<ArgumentOutOfRangeException>(() => reader.Skip(-1));
    }
}
