// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Server;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Services;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Server;

[TestFixture]
public class GetStreamStatisticsActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new GetStreamStatisticsAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WhenNeverEntered_ReturnsZeroedStatistics()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new GetStreamStatisticsAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(action.Depth, Is.EqualTo(0));
            Assert.That(action.Capacity, Is.EqualTo(0));
            Assert.That(action.Enqueued, Is.EqualTo(0));
            Assert.That(action.Dequeued, Is.EqualTo(0));
            Assert.That(action.Dropped, Is.EqualTo(0));
            Assert.That(action.Filtered, Is.EqualTo(0));
            Assert.That(action.MaxDepthSeen, Is.EqualTo(0));
        });
    }

    [Test]
    public void Execute_WithQueuedMessages_ReturnsRealStatistics()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport(), queueCapacity: 64);
        session.Client.Queue!.Clear();
        var baseline = session.Client.Queue!.GetStatistics();
        session.Client.Queue!.Enqueue(new ProbeMessage());
        session.Client.Queue!.Enqueue(new ProbeMessage());
        var action = new GetStreamStatisticsAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(action.Depth, Is.EqualTo(2));
            Assert.That(action.Capacity, Is.EqualTo(64));
            Assert.That(action.Enqueued, Is.EqualTo(baseline.Enqueued + 2));
            Assert.That(action.Dequeued, Is.EqualTo(baseline.Dequeued));
            Assert.That(action.Dropped, Is.EqualTo(baseline.Dropped));
            Assert.That(action.Filtered, Is.EqualTo(baseline.Filtered));
            Assert.That(action.MaxDepthSeen, Is.EqualTo(Math.Max(baseline.MaxDepthSeen, 2)));
        });
    }
}
