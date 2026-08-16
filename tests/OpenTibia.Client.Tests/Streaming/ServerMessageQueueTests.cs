// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Streaming;

[TestFixture]
public class ServerMessageQueueTests
{
    private static IProtocolMessage Message(byte opcode)
    {
        return new PayloadlessMessage(opcode);
    }

    [Test]
    public void DequeueBatch_ReturnsQueuedMessagesInOrder()
    {
        var queue = new ServerMessageQueue(8);
        queue.Enqueue(Message(1));
        queue.Enqueue(Message(2));

        var batch = queue.DequeueBatch(10, TimeSpan.Zero);

        Assert.Multiple(() =>
        {
            Assert.That(batch, Has.Count.EqualTo(2));
            Assert.That(batch[0].Opcode, Is.EqualTo(1));
            Assert.That(batch[1].Opcode, Is.EqualTo(2));
        });
    }

    [Test]
    public void DequeueBatch_HonoursMaxCount()
    {
        var queue = new ServerMessageQueue(8);
        queue.Enqueue(Message(1));
        queue.Enqueue(Message(2));
        queue.Enqueue(Message(3));

        var batch = queue.DequeueBatch(2, TimeSpan.Zero);

        Assert.Multiple(() =>
        {
            Assert.That(batch, Has.Count.EqualTo(2));
            Assert.That(queue.Depth, Is.EqualTo(1));
        });
    }

    [Test]
    public void DequeueBatch_WhenEmpty_WaitsThenReturnsEmpty()
    {
        var queue = new ServerMessageQueue(4);
        var stopwatch = Stopwatch.StartNew();

        var batch = queue.DequeueBatch(4, TimeSpan.FromMilliseconds(60));

        stopwatch.Stop();

        Assert.Multiple(() =>
        {
            Assert.That(batch, Is.Empty);
            Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(30));
        });
    }

    [Test]
    public void DequeueBatch_WhenProducerArrivesDuringWait_ReturnsTheMessage()
    {
        var queue = new ServerMessageQueue(4);
        var producer = new Thread(() =>
        {
            Thread.Sleep(30);
            queue.Enqueue(Message(0xAA));
        }) { IsBackground = true };

        producer.Start();
        var batch = queue.DequeueBatch(4, TimeSpan.FromMilliseconds(900));
        producer.Join();

        Assert.That(batch, Has.Count.EqualTo(1));
    }

    [Test]
    public void DequeueBatch_AfterComplete_DoesNotWait()
    {
        var queue = new ServerMessageQueue(4);
        queue.Complete();
        var stopwatch = Stopwatch.StartNew();

        var batch = queue.DequeueBatch(4, TimeSpan.FromMilliseconds(800));

        stopwatch.Stop();

        Assert.Multiple(() =>
        {
            Assert.That(batch, Is.Empty);
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(400));
        });
    }

    [Test]
    public void Enqueue_BeyondCapacity_CountsDropsAndKeepsNewest()
    {
        var queue = new ServerMessageQueue(2);
        queue.Enqueue(Message(1));
        queue.Enqueue(Message(2));
        queue.Enqueue(Message(3));

        var statistics = queue.GetStatistics();
        var batch = queue.DequeueBatch(10, TimeSpan.Zero);

        Assert.Multiple(() =>
        {
            Assert.That(statistics.Dropped, Is.EqualTo(1));
            Assert.That(statistics.Enqueued, Is.EqualTo(3));
            Assert.That(statistics.MaxDepthSeen, Is.EqualTo(2));
            Assert.That(batch[0].Opcode, Is.EqualTo(2));
            Assert.That(batch[1].Opcode, Is.EqualTo(3));
        });
    }

    [Test]
    public void GetStatistics_ReportsDepthCapacityAndCounters()
    {
        var queue = new ServerMessageQueue(4);
        queue.Enqueue(Message(1));
        queue.RecordFiltered();
        queue.DequeueBatch(1, TimeSpan.Zero);

        var statistics = queue.GetStatistics();

        Assert.Multiple(() =>
        {
            Assert.That(statistics.Capacity, Is.EqualTo(4));
            Assert.That(statistics.Depth, Is.Zero);
            Assert.That(statistics.Dequeued, Is.EqualTo(1));
            Assert.That(statistics.Filtered, Is.EqualTo(1));
            Assert.That(statistics.ToString(), Does.Contain("dropped 0"));
        });
    }

    [Test]
    public void Clear_DiscardsPendingMessages()
    {
        var queue = new ServerMessageQueue(4);
        queue.Enqueue(Message(1));
        queue.Enqueue(Message(2));

        Assert.Multiple(() =>
        {
            Assert.That(queue.Clear(), Is.EqualTo(2));
            Assert.That(queue.Depth, Is.Zero);
        });
    }

    [Test]
    public void Capacity_ReportsConfiguredCapacity()
    {
        Assert.That(new ServerMessageQueue(7).Capacity, Is.EqualTo(7));
    }

    [Test]
    public void ClampTimeout_AboveCeiling_ReturnsCeiling()
    {
        Assert.That(
            ServerMessageQueue.ClampTimeout(TimeSpan.FromSeconds(30)),
            Is.EqualTo(ServerMessageQueue.MaxSliceTimeout));
    }

    [Test]
    public void ClampTimeout_BelowZero_ReturnsZero()
    {
        Assert.That(ServerMessageQueue.ClampTimeout(TimeSpan.FromMilliseconds(-5)), Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void ClampTimeout_WithinRange_IsUnchanged()
    {
        var timeout = TimeSpan.FromMilliseconds(250);

        Assert.That(ServerMessageQueue.ClampTimeout(timeout), Is.EqualTo(timeout));
    }

    [Test]
    public void Enqueue_WithNullMessage_Throws()
    {
        var queue = new ServerMessageQueue(2);

        Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null!));
    }

    [Test]
    public void DequeueBatch_WithNonPositiveMaxCount_Throws()
    {
        var queue = new ServerMessageQueue(2);

        Assert.Throws<ArgumentOutOfRangeException>(() => queue.DequeueBatch(0, TimeSpan.Zero));
    }

    [Test]
    public void SustainedProducerAndConsumer_LoseNothingBeyondCountedDrops()
    {
        const int total = 5000;
        var queue = new ServerMessageQueue(4096);
        var received = 0;

        var producer = new Thread(() =>
        {
            for (var index = 0; index < total; index++)
            {
                queue.Enqueue(Message((byte)(index % 256)));
            }

            queue.Complete();
        }) { IsBackground = true };

        producer.Start();

        var deadline = Stopwatch.StartNew();
        while (deadline.Elapsed < TimeSpan.FromSeconds(20))
        {
            var batch = queue.DequeueBatch(128, TimeSpan.FromMilliseconds(20));
            received += batch.Count;

            var snapshot = queue.GetStatistics();
            if (snapshot.Enqueued == total && snapshot.Depth == 0)
            {
                break;
            }
        }

        producer.Join();
        var statistics = queue.GetStatistics();

        Assert.Multiple(() =>
        {
            Assert.That(statistics.Enqueued, Is.EqualTo(total));
            Assert.That(received + statistics.Dropped, Is.EqualTo(total),
                "Every message must be either delivered or explicitly counted as dropped.");
        });
    }
}
