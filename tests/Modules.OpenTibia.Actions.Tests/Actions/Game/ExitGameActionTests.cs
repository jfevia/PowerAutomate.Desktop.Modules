// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Game;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Game;

[TestFixture]
public class ExitGameActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new ExitGameAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WhileInGame_LogsOutAndDisconnects()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new ExitGameAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.That(session.Client.State, Is.EqualTo(ConnectionState.Disconnected));
    }

    [Test]
    public void Execute_WhenNeverEntered_DisconnectsWithoutThrowing()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new ExitGameAction { GameSession = session };

        Assert.DoesNotThrow(() => action.Execute(new ActionContext()));
        Assert.That(session.Client.State, Is.EqualTo(ConnectionState.Disconnected));
    }
}
