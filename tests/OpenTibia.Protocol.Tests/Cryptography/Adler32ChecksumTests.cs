// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Cryptography;

[TestFixture]
public class Adler32ChecksumTests
{
    [Test]
    public void Compute_WithEmptyBuffer_ReturnsSeed()
    {
        Assert.That(Adler32Checksum.Compute(Array.Empty<byte>()), Is.EqualTo(1u));
    }

    [Test]
    public void Compute_WithSingleByte_MatchesDefinition()
    {
        // low = 1 + 'a' = 98, high = 98 => (98 << 16) | 98
        Assert.That(Adler32Checksum.Compute(new[] { (byte)'a' }), Is.EqualTo((98u << 16) | 98u));
    }

    [Test]
    public void Compute_WithKnownAsciiInput_MatchesReferenceValue()
    {
        var value = Adler32Checksum.Compute(new byte[] { 0x57, 0x69, 0x6B, 0x69, 0x70, 0x65, 0x64, 0x69, 0x61 });

        Assert.That(value, Is.EqualTo(0x11E60398u));
    }

    [Test]
    public void Compute_WithRange_HonoursOffsetAndCount()
    {
        var buffer = new byte[] { 0xFF, (byte)'a', 0xFF };

        Assert.That(Adler32Checksum.Compute(buffer, 1, 1), Is.EqualTo((98u << 16) | 98u));
    }

    [Test]
    public void Compute_WithInputLongerThanModulus_WrapsCorrectly()
    {
        var buffer = new byte[1024];
        for (var index = 0; index < buffer.Length; index++)
        {
            buffer[index] = 0xFF;
        }

        Assert.That(Adler32Checksum.Compute(buffer), Is.Not.Zero);
    }

    [Test]
    public void Compute_WithNullBuffer_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Adler32Checksum.Compute(null!));
    }

    [Test]
    public void Compute_WithNullBufferAndRange_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Adler32Checksum.Compute(null!, 0, 0));
    }

    [Test]
    public void Compute_WithNegativeOffset_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Adler32Checksum.Compute(new byte[1], -1, 0));
    }

    [Test]
    public void Compute_WithNegativeCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Adler32Checksum.Compute(new byte[1], 0, -1));
    }

    [Test]
    public void Compute_WithRangeBeyondBuffer_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Adler32Checksum.Compute(new byte[1], 1, 1));
    }
}
