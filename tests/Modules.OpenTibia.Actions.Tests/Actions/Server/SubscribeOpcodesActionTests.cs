// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Server;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Server;

[TestFixture]
public class SubscribeOpcodesActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new SubscribeOpcodesAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WithNullOpcodes_ResetsFilter()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        session.Client.Filter.Allow(new byte[] { 1 });
        var action = new SubscribeOpcodesAction { GameSession = session, Opcodes = null! };

        action.Execute(new ActionContext());

        Assert.That(session.Client.Filter.AllowAll, Is.True);
    }

    [Test]
    public void Execute_WithEmptyOpcodes_ResetsFilter()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        session.Client.Filter.Allow(new byte[] { 1 });
        var action = new SubscribeOpcodesAction { GameSession = session, Opcodes = new List<int>() };

        action.Execute(new ActionContext());

        Assert.That(session.Client.Filter.AllowAll, Is.True);
    }

    [Test]
    public void Execute_WithOpcodes_AllowsOnlyThose()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new SubscribeOpcodesAction { GameSession = session, Opcodes = new List<int> { 0x64, 0xA0 } };

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(session.Client.Filter.AllowAll, Is.False);
            Assert.That(session.Client.Filter.IsAllowed(0x64), Is.True);
            Assert.That(session.Client.Filter.IsAllowed(0xA0), Is.True);
            Assert.That(session.Client.Filter.IsAllowed(0x01), Is.False);
        });
    }

    [Test]
    public void Execute_WithOutOfRangeOpcode_ThrowsInvalidArgument()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new SubscribeOpcodesAction { GameSession = session, Opcodes = new List<int> { 999 } };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
    }
}
