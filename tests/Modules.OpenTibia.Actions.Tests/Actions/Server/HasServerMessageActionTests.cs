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
public class HasServerMessageActionTests
{
    [Test]
    public void Execute_WithNullSession_ReturnsFalse()
    {
        var action = new HasServerMessageAction();

        action.Execute(new ActionContext());

        Assert.That(action.HasMessage, Is.False);
    }

    [Test]
    public void Execute_WhenNeverEntered_ReturnsFalse()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new HasServerMessageAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.That(action.HasMessage, Is.False);
    }

    [Test]
    public void Execute_WithEmptyQueue_ReturnsFalse()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        session.Client.Queue!.Clear();
        var action = new HasServerMessageAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.That(action.HasMessage, Is.False);
    }

    [Test]
    public void Execute_WithQueuedMessage_ReturnsTrue()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        session.Client.Queue!.Clear();
        session.Client.Queue!.Enqueue(new ProbeMessage());
        var action = new HasServerMessageAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.That(action.HasMessage, Is.True);
    }
}
