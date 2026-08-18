// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Server;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Services;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Server;

[TestFixture]
public class ReceiveServerMessagesActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new ReceiveServerMessagesAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Execute_WithMaxCountNotPositive_ThrowsInvalidArgument(int maxCount)
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new ReceiveServerMessagesAction { GameSession = session, MaxCount = maxCount };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
    }

    [Test]
    public void Execute_WhenNeverEntered_ReturnsEmptyAndTimedOut()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new ReceiveServerMessagesAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(action.Count, Is.EqualTo(0));
            Assert.That(action.TimedOut, Is.True);
            Assert.That(action.Messages.Rows, Is.Empty);
        });
    }

    [Test]
    public void Execute_WithEmptyQueue_WaitsThenReturnsTimedOut()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        session.Client.Queue!.Clear();
        var action = new ReceiveServerMessagesAction { GameSession = session, SliceTimeoutMs = 20 };

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(action.Count, Is.EqualTo(0));
            Assert.That(action.TimedOut, Is.True);
        });
    }

    [Test]
    public void Execute_WithQueuedMessages_ReturnsThemAndCount()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        session.Client.Queue!.Clear();
        session.Client.Queue!.Enqueue(new ProbeMessage());
        session.Client.Queue!.Enqueue(new ProbeMessage());
        var action = new ReceiveServerMessagesAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(action.Count, Is.EqualTo(2));
            Assert.That(action.TimedOut, Is.False);
            Assert.That(action.Messages.Rows, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void Execute_WithSliceTimeoutAboveMax_ClampsToOneSecond()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        session.Client.Queue!.Clear();
        var action = new ReceiveServerMessagesAction { GameSession = session, SliceTimeoutMs = 3000 };
        var clock = Stopwatch.StartNew();

        action.Execute(new ActionContext());

        clock.Stop();
        Assert.Multiple(() =>
        {
            Assert.That(clock.Elapsed, Is.LessThan(TimeSpan.FromMilliseconds(1800)));
            Assert.That(clock.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(900)));
        });
    }
}
