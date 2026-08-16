// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Server;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Services;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Server;

[TestFixture]
public class ClearServerMessagesActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new ClearServerMessagesAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WhenNeverEntered_ReturnsZero()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new ClearServerMessagesAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.That(action.Cleared, Is.EqualTo(0));
    }

    [Test]
    public void Execute_WithQueuedMessages_ClearsAndReturnsCount()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        session.Client.Queue!.Clear();
        session.Client.Queue!.Enqueue(new ProbeMessage());
        session.Client.Queue!.Enqueue(new ProbeMessage());
        var action = new ClearServerMessagesAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(action.Cleared, Is.EqualTo(2));
            Assert.That(session.Client.Queue!.Depth, Is.EqualTo(0));
        });
    }
}
