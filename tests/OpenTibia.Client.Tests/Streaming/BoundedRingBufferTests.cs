// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Streaming;

[TestFixture]
public class BoundedRingBufferTests
{
    [Test]
    public void Enqueue_WithinCapacity_ReportsNoDrop()
    {
        var buffer = new BoundedRingBuffer<int>(3);

        Assert.Multiple(() =>
        {
            Assert.That(buffer.Enqueue(1), Is.True);
            Assert.That(buffer.Count, Is.EqualTo(1));
            Assert.That(buffer.Capacity, Is.EqualTo(3));
        });
    }

    [Test]
    public void TryDequeue_ReturnsItemsInFifoOrder()
    {
        var buffer = new BoundedRingBuffer<int>(3);
        buffer.Enqueue(1);
        buffer.Enqueue(2);

        Assert.Multiple(() =>
        {
            Assert.That(buffer.TryDequeue(out var first), Is.True);
            Assert.That(first, Is.EqualTo(1));
            Assert.That(buffer.TryDequeue(out var second), Is.True);
            Assert.That(second, Is.EqualTo(2));
        });
    }

    [Test]
    public void TryDequeue_WhenEmpty_ReturnsFalse()
    {
        var buffer = new BoundedRingBuffer<int>(2);

        Assert.Multiple(() =>
        {
            Assert.That(buffer.TryDequeue(out var item), Is.False);
            Assert.That(item, Is.Zero);
        });
    }

    [Test]
    public void Enqueue_BeyondCapacity_DropsOldestAndKeepsOrder()
    {
        var buffer = new BoundedRingBuffer<int>(3);
        buffer.Enqueue(1);
        buffer.Enqueue(2);
        buffer.Enqueue(3);

        var accepted = buffer.Enqueue(4);

        Assert.Multiple(() =>
        {
            Assert.That(accepted, Is.False, "Overwriting the oldest entry must be reported as a drop.");
            Assert.That(buffer.Count, Is.EqualTo(3));
            Assert.That(buffer.TryDequeue(out var first), Is.True);
            Assert.That(first, Is.EqualTo(2), "The oldest entry should have been discarded.");
            Assert.That(buffer.TryDequeue(out var second), Is.True);
            Assert.That(second, Is.EqualTo(3));
            Assert.That(buffer.TryDequeue(out var third), Is.True);
            Assert.That(third, Is.EqualTo(4));
        });
    }

    [Test]
    public void Enqueue_AfterWrapAround_StillPreservesOrder()
    {
        var buffer = new BoundedRingBuffer<int>(2);
        buffer.Enqueue(1);
        buffer.Enqueue(2);
        buffer.TryDequeue(out _);
        buffer.Enqueue(3);
        buffer.TryDequeue(out _);
        buffer.Enqueue(4);

        Assert.Multiple(() =>
        {
            Assert.That(buffer.TryDequeue(out var first), Is.True);
            Assert.That(first, Is.EqualTo(3));
            Assert.That(buffer.TryDequeue(out var second), Is.True);
            Assert.That(second, Is.EqualTo(4));
        });
    }

    [Test]
    public void Clear_ReportsDiscardedCountAndEmpties()
    {
        var buffer = new BoundedRingBuffer<int>(3);
        buffer.Enqueue(1);
        buffer.Enqueue(2);

        Assert.Multiple(() =>
        {
            Assert.That(buffer.Clear(), Is.EqualTo(2));
            Assert.That(buffer.Count, Is.Zero);
        });
    }

    [Test]
    public void Clear_WhenEmpty_ReturnsZero()
    {
        var buffer = new BoundedRingBuffer<int>(2);

        Assert.That(buffer.Clear(), Is.Zero);
    }

    [Test]
    public void Constructor_WithNonPositiveCapacity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BoundedRingBuffer<int>(0));
    }
}
