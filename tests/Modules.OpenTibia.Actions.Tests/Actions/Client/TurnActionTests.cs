// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Client;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Client;

[TestFixture]
public class TurnActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new TurnAction { Direction = Direction.North };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WhenNotInGame_ThrowsNotConnected()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new TurnAction { GameSession = session, Direction = Direction.North };

        Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
    }

    [TestCase(Direction.North)]
    [TestCase(Direction.East)]
    [TestCase(Direction.South)]
    [TestCase(Direction.West)]
    public void Execute_WithACardinalDirection_Succeeds(Direction direction)
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateInGameSession(transport);
        transport.Written.Clear();
        var action = new TurnAction { GameSession = session, Direction = direction };

        action.Execute(new ActionContext());

        Assert.That(transport.Written, Has.Count.EqualTo(1));
    }

    [TestCase(Direction.NorthEast)]
    [TestCase(Direction.SouthEast)]
    [TestCase(Direction.SouthWest)]
    [TestCase(Direction.NorthWest)]
    public void Execute_WithADiagonalDirection_ThrowsProtocolError(Direction direction)
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new TurnAction { GameSession = session, Direction = direction };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("ProtocolErrorError"));
    }
}
