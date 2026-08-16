// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Services;

[TestFixture]
public class HexCodecTests
{
    [Test]
    public void Parse_WithNullHex_ReturnsEmptyArray()
    {
        Assert.That(HexCodec.Parse(null), Is.Empty);
    }

    [Test]
    public void Parse_WithEmptyHex_ReturnsEmptyArray()
    {
        Assert.That(HexCodec.Parse(string.Empty), Is.Empty);
    }

    [Test]
    public void Parse_WithOddLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => HexCodec.Parse("ABC"));
    }

    [Test]
    public void Parse_WithMixedCaseHexDigits_ReturnsCorrectBytes()
    {
        Assert.That(HexCodec.Parse("0aAF9bB1"), Is.EqualTo(new byte[] { 0x0A, 0xAF, 0x9B, 0xB1 }));
    }

    [Test]
    public void Parse_WithInvalidHexDigit_Throws()
    {
        Assert.Throws<ArgumentException>(() => HexCodec.Parse("0G"));
    }
}
