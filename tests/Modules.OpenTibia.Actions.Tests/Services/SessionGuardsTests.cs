// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Client;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Services;

[TestFixture]
public class SessionGuardsTests
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
    public void RequireLogin_WithNullSession_Throws()
    {
        var exception = Assert.Throws<ActionException>(() => SessionGuards.RequireLogin(null))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void RequireLogin_WithDisconnectedTransport_Throws()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateLoginSession(transport, "Rook");
        transport.Close();

        Assert.Throws<ActionException>(() => SessionGuards.RequireLogin(session));
    }

    [Test]
    public void RequireLogin_WithConnectedSession_ReturnsIt()
    {
        var session = SessionFactory.CreateLoginSession(new FakeSocketTransport(), "Rook");

        Assert.That(SessionGuards.RequireLogin(session), Is.SameAs(session));
    }

    [Test]
    public void RequireGame_WithNullSession_Throws()
    {
        var exception = Assert.Throws<ActionException>(() => SessionGuards.RequireGame(null))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void RequireGame_WithAnySession_ReturnsItRegardlessOfState()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());

        Assert.That(SessionGuards.RequireGame(session), Is.SameAs(session));
    }

    [Test]
    public void RequireInGame_WithNullSession_Throws()
    {
        Assert.Throws<ActionException>(() => SessionGuards.RequireInGame(null));
    }

    [Test]
    public void RequireInGame_WithWrongState_Throws()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());

        var exception = Assert.Throws<ActionException>(() => SessionGuards.RequireInGame(session))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void RequireInGame_WhenFaulted_Throws()
    {
        var session = SessionFactory.CreateInGameSessionThatWillFault(new FakeSocketTransport());
        WaitForFault(session);

        Assert.Throws<ActionException>(() => SessionGuards.RequireInGame(session));
    }

    [Test]
    public void RequireInGame_WithHealthySession_ReturnsIt()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());

        Assert.That(SessionGuards.RequireInGame(session), Is.SameAs(session));
    }
}
