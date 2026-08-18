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
using PowerAutomate.Desktop.OpenTibia.Client.Transport;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Game;

[TestFixture]
public class DisconnectActionTests
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
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new DisconnectAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WhileInGame_Disconnects()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new DisconnectAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.That(session.Client.State, Is.EqualTo(ConnectionState.Disconnected));
    }

    [Test]
    public void Execute_CalledTwice_IsIdempotent()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new DisconnectAction { GameSession = session };

        action.Execute(new ActionContext());

        Assert.DoesNotThrow(() => action.Execute(new ActionContext()));
        Assert.That(session.Client.State, Is.EqualTo(ConnectionState.Disconnected));
    }

    [Test]
    public void Execute_AfterAFault_IsSafe()
    {
        var session = SessionFactory.CreateInGameSessionThatWillFault(new FakeSocketTransport());
        WaitForFault(session);
        var action = new DisconnectAction { GameSession = session };

        Assert.DoesNotThrow(() => action.Execute(new ActionContext()));
        Assert.That(session.Client.State, Is.EqualTo(ConnectionState.Disconnected));
    }
}
