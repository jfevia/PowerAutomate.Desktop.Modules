// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Characters;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Characters;

[TestFixture]
public class GetCharacterListActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new GetCharacterListAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WithDisconnectedTransport_ThrowsNotConnected()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateLoginSession(transport, "Rook");
        transport.Close();
        var action = new GetCharacterListAction { LoginSession = session };

        Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
    }

    [Test]
    public void Execute_WithActiveSession_ReturnsTheCharacterTable()
    {
        var session = SessionFactory.CreateLoginSession(new FakeSocketTransport(), "Rook", "Knight");
        var action = new GetCharacterListAction { LoginSession = session };

        action.Execute(new ActionContext());

        Assert.That(action.Characters.Rows, Has.Count.EqualTo(2));
    }
}
