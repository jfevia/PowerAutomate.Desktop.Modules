// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Framing;

[TestFixture]
public class PacketWriterTests
{
    [Test]
    public void WriteByte_AppendsValue()
    {
        var writer = new PacketWriter();

        writer.WriteByte(0xAB);

        Assert.Multiple(() =>
        {
            Assert.That(writer.Length, Is.EqualTo(1));
            Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { 0xAB }));
        });
    }

    [Test]
    public void WriteUInt16_WritesLittleEndian()
    {
        var writer = new PacketWriter();

        writer.WriteUInt16(0x1234);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { 0x34, 0x12 }));
    }

    [Test]
    public void WriteUInt32_WritesLittleEndian()
    {
        var writer = new PacketWriter();

        writer.WriteUInt32(0x12345678);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { 0x78, 0x56, 0x34, 0x12 }));
    }

    [Test]
    public void WriteUInt64_WritesLittleEndian()
    {
        var writer = new PacketWriter();

        writer.WriteUInt64(0x0123456789ABCDEF);

        Assert.That(writer.ToArray(),
            Is.EqualTo(new byte[] { 0xEF, 0xCD, 0xAB, 0x89, 0x67, 0x45, 0x23, 0x01 }));
    }

    [Test]
    public void WriteBytes_AppendsWholeArray()
    {
        var writer = new PacketWriter();

        writer.WriteBytes(new byte[] { 1, 2 });

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { 1, 2 }));
    }

    [Test]
    public void WriteBytes_WithRange_AppendsOnlyThatRange()
    {
        var writer = new PacketWriter();

        writer.WriteBytes(new byte[] { 9, 1, 2, 9 }, 1, 2);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { 1, 2 }));
    }

    [Test]
    public void WriteString_WritesLengthPrefixedLatin1()
    {
        var writer = new PacketWriter();

        writer.WriteString("hi");

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { 0x02, 0x00, 0x68, 0x69 }));
    }

    [Test]
    public void EnsureCapacity_GrowsRepeatedlyForLargePayloads()
    {
        var writer = new PacketWriter(1);

        writer.WriteBytes(new byte[64]);

        Assert.That(writer.Length, Is.EqualTo(64));
    }

    [Test]
    public void EnsureCapacity_WithinExistingCapacity_DoesNotGrow()
    {
        var writer = new PacketWriter(8);

        writer.WriteBytes(new byte[4]);

        Assert.That(writer.Length, Is.EqualTo(4));
    }

    [Test]
    public void Constructor_WithNonPositiveCapacity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PacketWriter(0));
    }

    [Test]
    public void WriteBytes_WithNullArray_Throws()
    {
        var writer = new PacketWriter();

        Assert.Throws<ArgumentNullException>(() => writer.WriteBytes(null!));
    }

    [Test]
    public void WriteBytes_WithNullArrayAndRange_Throws()
    {
        var writer = new PacketWriter();

        Assert.Throws<ArgumentNullException>(() => writer.WriteBytes(null!, 0, 0));
    }

    [Test]
    public void WriteBytes_WithNegativeOffset_Throws()
    {
        var writer = new PacketWriter();

        Assert.Throws<ArgumentOutOfRangeException>(() => writer.WriteBytes(new byte[1], -1, 0));
    }

    [Test]
    public void WriteBytes_WithNegativeCount_Throws()
    {
        var writer = new PacketWriter();

        Assert.Throws<ArgumentOutOfRangeException>(() => writer.WriteBytes(new byte[1], 0, -1));
    }

    [Test]
    public void WriteBytes_WithRangeBeyondArray_Throws()
    {
        var writer = new PacketWriter();

        Assert.Throws<ArgumentOutOfRangeException>(() => writer.WriteBytes(new byte[1], 1, 1));
    }

    [Test]
    public void WriteString_WithNullValue_Throws()
    {
        var writer = new PacketWriter();

        Assert.Throws<ArgumentNullException>(() => writer.WriteString(null!));
    }
}
