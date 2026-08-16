// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Client;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Client;

[TestFixture]
public class SendTalkActionTests
{
    [Test]
    public void Execute_WithNullSession_ThrowsNotConnected()
    {
        var action = new SendTalkAction { Text = "hello" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WhenNotInGame_ThrowsNotConnected()
    {
        var session = SessionFactory.CreateDisconnectedGameSession(new FakeSocketTransport());
        var action = new SendTalkAction { GameSession = session, Text = "hello" };

        Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
    }

    [Test]
    public void Execute_WithSayType_SendsTheMessage()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateInGameSession(transport);
        transport.Written.Clear();
        var action = new SendTalkAction { GameSession = session, Type = SpeakType.Say, Text = "hello" };

        action.Execute(new ActionContext());

        Assert.That(transport.Written, Has.Count.EqualTo(1));
    }

    [Test]
    public void Execute_WithPrivateTypeAndReceiver_SendsTheMessage()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateInGameSession(transport);
        transport.Written.Clear();
        var action = new SendTalkAction
        {
            GameSession = session,
            Type = SpeakType.Private,
            ReceiverName = "Friend",
            Text = "hi there"
        };

        action.Execute(new ActionContext());

        Assert.That(transport.Written, Has.Count.EqualTo(1));
    }

    [Test]
    public void Execute_WithPrivateTypeAndNoReceiver_ThrowsProtocolError()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new SendTalkAction { GameSession = session, Type = SpeakType.Private, Text = "hi" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("ProtocolErrorError"));
    }

    [Test]
    public void Execute_WithChannelTypeAndId_SendsTheMessage()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateInGameSession(transport);
        transport.Written.Clear();
        var action = new SendTalkAction
        {
            GameSession = session,
            Type = SpeakType.ChannelYellow,
            ChannelId = 5,
            Text = "trade please"
        };

        action.Execute(new ActionContext());

        Assert.That(transport.Written, Has.Count.EqualTo(1));
    }

    [Test]
    public void Execute_WithChannelTypeAndNoId_ThrowsProtocolError()
    {
        var session = SessionFactory.CreateInGameSession(new FakeSocketTransport());
        var action = new SendTalkAction { GameSession = session, Type = SpeakType.ChannelYellow, Text = "trade" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("ProtocolErrorError"));
    }

    [Test]
    public void Execute_WithNullText_SendsEmptyText()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateInGameSession(transport);
        transport.Written.Clear();
        var action = new SendTalkAction { GameSession = session, Type = SpeakType.Say, Text = null! };

        action.Execute(new ActionContext());

        Assert.That(transport.Written, Has.Count.EqualTo(1));
    }
}
