// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Client;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Client;

[TestFixture]
public class LookActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new LookAction { ItemId = 100 };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WhenNotInGame_ThrowsNotConnected()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new LookAction { GameSession = session, ItemId = 100 };

        Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
    }

    [TestCase(70000, 100, 0, 100, 0)]
    [TestCase(100, 70000, 0, 100, 0)]
    [TestCase(100, 100, 300, 100, 0)]
    [TestCase(100, 100, 0, 70000, 0)]
    [TestCase(100, 100, 0, 100, 300)]
    public void Execute_WithOutOfRangeField_ThrowsInvalidArgument(int x, int y, int z, int itemId, int stackPosition)
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new LookAction
        {
            GameSession = session,
            X = x,
            Y = y,
            Z = z,
            ItemId = itemId,
            StackPosition = stackPosition
        };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
    }

    [Test]
    public void Execute_WithValidFields_SendsTheMessage()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateInGameSession(transport);
        transport.Written.Clear();
        var action = new LookAction
        {
            GameSession = session,
            X = 100,
            Y = 100,
            Z = 7,
            ItemId = 100,
            StackPosition = 1
        };

        action.Execute(new ActionContext());

        Assert.That(transport.Written, Has.Count.EqualTo(1));
    }
}
