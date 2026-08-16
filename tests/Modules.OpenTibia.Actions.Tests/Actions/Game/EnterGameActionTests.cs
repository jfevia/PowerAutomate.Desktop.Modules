// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Game;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Game;

[TestFixture]
public class EnterGameActionTests
{
    private static EnterGameAction CreateAction(FakeSocketTransport transport)
    {
        return new EnterGameAction(() => transport, () => SessionFactory.GameKey)
        {
            LoginSession = SessionFactory.CreateLoginSession(new FakeSocketTransport(), "Rook"),
            Character = new TibiaCharacter("Rook", "Antica", "game.example", 7172),
            QueueCapacity = 64,
            TimeoutMs = 15000
        };
    }

    [Test]
    public void Execute_WithNullLoginSession_ThrowsNotConnected()
    {
        var action = new EnterGameAction(() => new FakeSocketTransport(), () => SessionFactory.GameKey)
        {
            Character = new TibiaCharacter("Rook", "Antica", "game.example", 7172)
        };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
    }

    [Test]
    public void Execute_WithNullCharacter_ThrowsInvalidArgument()
    {
        var action = CreateAction(new FakeSocketTransport());
        action.Character = null!;

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
    }

    [Test]
    public void Execute_WithValidCharacter_PopulatesGameSession()
    {
        var transport = new FakeSocketTransport();
        transport.BlockWhenExhausted();
        transport.EnqueueRead(Frames.GameChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(Frames.GamePendingStatePayload(), SessionFactory.GameKey));
        var action = CreateAction(transport);

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(action.GameSession, Is.Not.Null);
            Assert.That(action.GameSession.CharacterName, Is.EqualTo("Rook"));
            Assert.That(action.GameSession.World, Is.EqualTo("Antica"));
        });
    }

    [Test]
    public void Execute_WhenServerRejects_ThrowsAuthenticationFailed()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(Frames.GameChallengeFrame());
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(Frames.GameLoginErrorPayload("account banned"), SessionFactory.GameKey));
        var action = CreateAction(transport);

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("AuthenticationFailedError"));
    }

    [Test]
    public void Execute_WhenConnectThrowsSocketException_ThrowsConnectionFailed()
    {
        var transport = new FakeSocketTransport { ConnectFailure = new SocketException((int)SocketError.ConnectionRefused) };
        var action = CreateAction(transport);

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("ConnectionFailedError"));
    }

    [Test]
    public void Execute_WhenServerClosesEarly_ThrowsProtocolError()
    {
        var action = CreateAction(new FakeSocketTransport());

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("ProtocolErrorError"));
    }

    [Test]
    public void Execute_WhenNothingArrivesInTime_ThrowsTimeout()
    {
        var action = CreateAction(new FakeSocketTransport());
        action.TimeoutMs = 0;

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("TimeoutError"));
    }

    [Test]
    public void Constructor_WithNullTransportFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new EnterGameAction(null!, () => SessionFactory.GameKey));
    }

    [Test]
    public void Constructor_WithNullKeyFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new EnterGameAction(() => new FakeSocketTransport(), null!));
    }

    [Test]
    public void DefaultConstructor_WiresARealTransportFactory()
    {
        var action = new EnterGameAction();

        var field = typeof(EnterGameAction).GetField("_transportFactory", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var factory = (Func<ISocketTransport>)field.GetValue(action)!;

        using var transport = factory();

        Assert.That(transport, Is.InstanceOf<ISocketTransport>());
    }
}
