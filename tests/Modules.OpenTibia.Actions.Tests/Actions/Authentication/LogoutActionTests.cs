// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Authentication;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Authentication;

[TestFixture]
public class LogoutActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new LogoutAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WithActiveSession_ClosesTheTransport()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateLoginSession(transport, "Rook");
        var action = new LogoutAction { LoginSession = session };

        action.Execute(new ActionContext());

        Assert.That(transport.IsConnected, Is.False);
    }

    [Test]
    public void Execute_CalledTwice_IsIdempotent()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateLoginSession(transport, "Rook");
        var action = new LogoutAction { LoginSession = session };

        action.Execute(new ActionContext());

        Assert.DoesNotThrow(() => action.Execute(new ActionContext()));
        Assert.That(transport.IsConnected, Is.False);
    }
}
