// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests;

[TestFixture]
public class Latin1Tests
{
    [Test]
    public void GetString_WithAsciiRange_DecodesEveryByte()
    {
        var buffer = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };

        Assert.That(Latin1.GetString(buffer, 0, buffer.Length), Is.EqualTo("Hello"));
    }

    [Test]
    public void GetString_WithOffset_DecodesOnlyTheRequestedRange()
    {
        var buffer = new byte[] { 0x41, 0x42, 0x43, 0x44 };

        Assert.That(Latin1.GetString(buffer, 1, 2), Is.EqualTo("BC"));
    }

    [Test]
    public void GetString_WithHighBytes_MapsToMatchingCodePoints()
    {
        var buffer = new byte[] { 0xE9, 0xFF };

        Assert.That(Latin1.GetString(buffer, 0, 2), Is.EqualTo("\u00E9\u00FF"));
    }

    [Test]
    public void GetString_WithZeroCount_ReturnsEmpty()
    {
        Assert.That(Latin1.GetString(new byte[] { 1 }, 0, 0), Is.EqualTo(string.Empty));
    }

    [Test]
    public void GetString_WithNullBuffer_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Latin1.GetString(null!, 0, 0));
    }

    [Test]
    public void GetBytes_WithLatinCharacters_EncodesOneBytePerCharacter()
    {
        Assert.That(Latin1.GetBytes("A\u00FF"), Is.EqualTo(new byte[] { 0x41, 0xFF }));
    }

    [Test]
    public void GetBytes_WithCharacterAboveLatinRange_SubstitutesReplacement()
    {
        Assert.That(Latin1.GetBytes("\u20AC"), Is.EqualTo(new[] { Latin1.ReplacementByte }));
    }

    [Test]
    public void GetBytes_WithEmptyString_ReturnsEmptyArray()
    {
        Assert.That(Latin1.GetBytes(string.Empty), Is.Empty);
    }

    [Test]
    public void GetBytes_WithNullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Latin1.GetBytes(null!));
    }
}
