// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Client;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Client;

[TestFixture]
public class AutoWalkActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new AutoWalkAction { Directions = new List<Direction> { Direction.North } };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WhenNotInGame_ThrowsNotConnected()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new AutoWalkAction { GameSession = session, Directions = new List<Direction> { Direction.North } };

        Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
    }

    [Test]
    public void Execute_WithNullDirections_ThrowsInvalidArgument()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new AutoWalkAction { GameSession = session, Directions = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
    }

    [Test]
    public void Execute_WithEmptyDirections_ThrowsProtocolError()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new AutoWalkAction { GameSession = session, Directions = new List<Direction>() };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("ProtocolErrorError"));
    }

    [Test]
    public void Execute_WithAPath_SendsTheMessage()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateInGameSession(transport);
        transport.Written.Clear();
        var action = new AutoWalkAction
        {
            GameSession = session,
            Directions = new List<Direction> { Direction.North, Direction.East }
        };

        action.Execute(new ActionContext());

        Assert.That(transport.Written, Has.Count.EqualTo(1));
    }
}
