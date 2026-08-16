// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Streaming;

[TestFixture]
public class OpcodeFilterTests
{
    [Test]
    public void IsAllowed_ByDefault_AllowsEverything()
    {
        var filter = new OpcodeFilter();

        Assert.Multiple(() =>
        {
            Assert.That(filter.AllowAll, Is.True);
            Assert.That(filter.IsAllowed(0x00), Is.True);
            Assert.That(filter.IsAllowed(0xFF), Is.True);
        });
    }

    [Test]
    public void Allow_RestrictsToTheSuppliedOpcodes()
    {
        var filter = new OpcodeFilter();

        filter.Allow(new byte[] { 0xAA, 0xB4 });

        Assert.Multiple(() =>
        {
            Assert.That(filter.AllowAll, Is.False);
            Assert.That(filter.IsAllowed(0xAA), Is.True);
            Assert.That(filter.IsAllowed(0xB4), Is.True);
            Assert.That(filter.IsAllowed(0x64), Is.False);
        });
    }

    [Test]
    public void Reset_RestoresAllowAll()
    {
        var filter = new OpcodeFilter();
        filter.Allow(new byte[] { 0xAA });

        filter.Reset();

        Assert.Multiple(() =>
        {
            Assert.That(filter.AllowAll, Is.True);
            Assert.That(filter.IsAllowed(0x64), Is.True);
        });
    }

    [Test]
    public void Allow_WithNullSequence_Throws()
    {
        var filter = new OpcodeFilter();

        Assert.Throws<ArgumentNullException>(() => filter.Allow(null!));
    }

    [Test]
    public void Allow_WithEmptySequence_Throws()
    {
        var filter = new OpcodeFilter();

        Assert.Throws<ArgumentException>(() => filter.Allow(Array.Empty<byte>()));
    }
}
