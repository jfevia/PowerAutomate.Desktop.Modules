// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Client;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Types;

[TestFixture]
public class TibiaGameSessionTests
{
    private static void WaitForFault(TibiaGameSession session)
    {
        var clock = Stopwatch.StartNew();
        while (session.Client.FaultReason == null && clock.Elapsed < TimeSpan.FromSeconds(5))
        {
            Thread.Sleep(10);
        }
    }

    [Test]
    public void Constructor_KeepsEveryVisibleProperty()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport(), "Rook", "Antica");

        Assert.Multiple(() =>
        {
            Assert.That(session.CharacterName, Is.EqualTo("Rook"));
            Assert.That(session.World, Is.EqualTo("Antica"));
        });
    }

    [Test]
    public void ToString_DescribesTheSession()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport(), "Rook", "Antica");

        Assert.That(session.ToString(), Is.EqualTo("Rook @ Antica (InGame)"));
    }

    [Test]
    public void DoesNotImplementICloneable()
    {
        Assert.That(typeof(ICloneable).IsAssignableFrom(typeof(TibiaGameSession)), Is.False);
    }

    [Test]
    public void State_WhenNeverEntered_ReportsDisconnected()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());

        Assert.That(session.State, Is.EqualTo("Disconnected"));
    }

    [Test]
    public void State_WhenReaderFaults_ReportsFaulted()
    {
        var session = SessionFactory.CreateInGameSessionThatWillFault(new FakeSocketTransport());

        WaitForFault(session);

        Assert.That(session.State, Is.EqualTo("Faulted"));
    }

    [Test]
    public void QueueDepth_WhenNeverEntered_IsZero()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());

        Assert.That(session.QueueDepth, Is.EqualTo(0));
    }

    [Test]
    public void QueueDepth_ReflectsTheLiveQueue()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());

        Assert.That(session.QueueDepth, Is.EqualTo(session.Client.Queue!.Depth));
    }

    [Test]
    public void Dropped_WhenNeverEntered_IsZero()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());

        Assert.That(session.Dropped, Is.EqualTo(0));
    }

    [Test]
    public void Dropped_ReflectsQueueStatistics()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport(), queueCapacity: 1);

        // Overflow the one-slot queue so the underlying statistics record a drop.
        session.Client.Queue!.Enqueue(new ProbeMessage());
        session.Client.Queue!.Enqueue(new ProbeMessage());

        Assert.That(session.Dropped, Is.EqualTo(session.Client.Queue!.GetStatistics().Dropped));
        Assert.That(session.Dropped, Is.GreaterThan(0));
    }

    [Test]
    public void Constructor_WithNullClient_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TibiaGameSession(null!, "Rook", "Antica"));
    }

    [Test]
    public void Constructor_WithNullCharacterName_Throws()
    {
        var client = new TibiaGameClient(new FakeSocketTransport());

        Assert.Throws<ArgumentNullException>(() => new TibiaGameSession(client, null!, "Antica"));
    }

    [Test]
    public void Constructor_WithNullWorld_Throws()
    {
        var client = new TibiaGameClient(new FakeSocketTransport());

        Assert.Throws<ArgumentNullException>(() => new TibiaGameSession(client, "Rook", null!));
    }

    private sealed class ProbeMessage : PowerAutomate.Desktop.OpenTibia.Protocol.Messages.IProtocolMessage
    {
        public byte Opcode => 0x00;
    }
}
