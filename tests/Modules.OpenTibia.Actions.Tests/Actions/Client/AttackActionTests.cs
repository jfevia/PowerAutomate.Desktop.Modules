// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Client;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Client;

[TestFixture]
public class AttackActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new AttackAction { CreatureId = 42 };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WhenNotInGame_ThrowsNotConnected()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new AttackAction { GameSession = session, CreatureId = 42 };

        Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
    }

    [Test]
    public void Execute_WithNegativeCreatureId_ThrowsInvalidArgument()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new AttackAction { GameSession = session, CreatureId = -1 };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
    }

    [Test]
    public void Execute_WithValidCreatureId_SendsTheMessage()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateInGameSession(transport);
        transport.Written.Clear();
        var action = new AttackAction { GameSession = session, CreatureId = 1234 };

        action.Execute(new ActionContext());

        Assert.That(transport.Written, Has.Count.EqualTo(1));
    }
}
