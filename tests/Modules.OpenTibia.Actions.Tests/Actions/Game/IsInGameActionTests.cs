// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Game;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Game;

[TestFixture]
public class IsInGameActionTests
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
    public void Execute_WithNullSession_ReturnsFalse()
    {
        var action = new IsInGameAction();

        action.Execute(new ActionContext());

        Assert.That(action.IsInGame, Is.False);
    }

    [Test]
    public void Execute_WhenNeverEntered_ReturnsFalse()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new IsInGameAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.That(action.IsInGame, Is.False);
    }

    [Test]
    public void Execute_WhenFaulted_ReturnsFalse()
    {
        var session = SessionFactory.CreateInGameSessionThatWillFault(new FakeSocketTransport());
        WaitForFault(session);
        var action = new IsInGameAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.That(action.IsInGame, Is.False);
    }

    [Test]
    public void Execute_WhenHealthyAndInGame_ReturnsTrue()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new IsInGameAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.That(action.IsInGame, Is.True);
    }
}
