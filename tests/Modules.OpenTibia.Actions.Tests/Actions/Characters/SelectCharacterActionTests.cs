// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Characters;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Characters;

[TestFixture]
public class SelectCharacterActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new SelectCharacterAction { CharacterName = "Rook" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WithNullCharacterName_ThrowsInvalidArgument()
    {
        var session = SessionFactory.CreateLoginSession(new FakeSocketTransport(), "Rook");
        var action = new SelectCharacterAction { LoginSession = session, CharacterName = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
    }

    [Test]
    public void Execute_WithEmptyCharacterName_ThrowsInvalidArgument()
    {
        var session = SessionFactory.CreateLoginSession(new FakeSocketTransport(), "Rook");
        var action = new SelectCharacterAction { LoginSession = session, CharacterName = string.Empty };

        Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
    }

    [Test]
    public void Execute_WithUnknownCharacter_ThrowsCharacterNotFoundListingAvailableNames()
    {
        var session = SessionFactory.CreateLoginSession(new FakeSocketTransport(), "Rook", "Knight");
        var action = new SelectCharacterAction { LoginSession = session, CharacterName = "Ghost" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.Multiple(() =>
        {
            Assert.That(exception.Name, Is.EqualTo("CharacterNotFoundError"));
            Assert.That(exception.Message, Does.Contain("Rook"));
            Assert.That(exception.Message, Does.Contain("Knight"));
        });
    }

    [Test]
    public void Execute_WithMatchingCharacter_ReturnsItCaseInsensitively()
    {
        var session = SessionFactory.CreateLoginSession(new FakeSocketTransport(), "Rook");
        var action = new SelectCharacterAction { LoginSession = session, CharacterName = "rOOk" };

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(action.Character.Name, Is.EqualTo("Rook"));
            Assert.That(action.Character.World, Is.EqualTo("Antica"));
            Assert.That(action.Character.Host, Is.EqualTo("127.0.0.1"));
            Assert.That(action.Character.Port, Is.EqualTo(7172));
        });
    }
}
