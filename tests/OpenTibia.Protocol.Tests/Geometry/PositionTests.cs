// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Geometry;

[TestFixture]
public class PositionTests
{
    [Test]
    public void Constructor_KeepsCoordinates()
    {
        var position = new Position(100, 200, 7);

        Assert.Multiple(() =>
        {
            Assert.That(position.X, Is.EqualTo(100));
            Assert.That(position.Y, Is.EqualTo(200));
            Assert.That(position.Z, Is.EqualTo(7));
        });
    }

    [Test]
    public void Equals_WithIdenticalCoordinates_IsTrue()
    {
        Assert.That(new Position(1, 2, 3).Equals(new Position(1, 2, 3)), Is.True);
    }

    [Test]
    public void Equals_WithDifferentX_IsFalse()
    {
        Assert.That(new Position(1, 2, 3).Equals(new Position(9, 2, 3)), Is.False);
    }

    [Test]
    public void Equals_WithDifferentY_IsFalse()
    {
        Assert.That(new Position(1, 2, 3).Equals(new Position(1, 9, 3)), Is.False);
    }

    [Test]
    public void Equals_WithDifferentZ_IsFalse()
    {
        Assert.That(new Position(1, 2, 3).Equals(new Position(1, 2, 9)), Is.False);
    }

    [Test]
    public void Equals_WithMatchingBoxedPosition_IsTrue()
    {
        Assert.That(new Position(1, 2, 3).Equals((object)new Position(1, 2, 3)), Is.True);
    }

    [Test]
    public void Equals_WithForeignType_IsFalse()
    {
        Assert.That(new Position(1, 2, 3).Equals("nope"), Is.False);
    }

    [Test]
    public void GetHashCode_MatchesForEqualPositions()
    {
        Assert.That(new Position(4, 5, 6).GetHashCode(), Is.EqualTo(new Position(4, 5, 6).GetHashCode()));
    }

    [Test]
    public void EqualityOperator_ComparesByValue()
    {
        Assert.Multiple(() =>
        {
            Assert.That(new Position(1, 2, 3) == new Position(1, 2, 3), Is.True);
            Assert.That(new Position(1, 2, 3) != new Position(3, 2, 1), Is.True);
        });
    }

    [Test]
    public void ToString_RendersCoordinates()
    {
        Assert.That(new Position(1, 2, 3).ToString(), Is.EqualTo("(1, 2, 3)"));
    }
}

[TestFixture]
public class PositionCodecTests
{
    [Test]
    public void Read_ParsesLittleEndianCoordinates()
    {
        var reader = new PacketReader(new byte[] { 0x10, 0x27, 0x20, 0x4E, 0x07 });

        var position = PositionCodec.Read(reader);

        Assert.That(position, Is.EqualTo(new Position(10000, 20000, 7)));
    }

    [Test]
    public void Write_RoundTripsThroughRead()
    {
        var writer = new PacketWriter();
        var expected = new Position(1234, 4321, 5);

        PositionCodec.Write(writer, expected);

        Assert.That(PositionCodec.Read(new PacketReader(writer.ToArray())), Is.EqualTo(expected));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => PositionCodec.Read(null!));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => PositionCodec.Write(null!, default));
    }
}
