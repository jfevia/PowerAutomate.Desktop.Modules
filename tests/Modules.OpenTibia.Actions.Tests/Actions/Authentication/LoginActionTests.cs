// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Data;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Authentication;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Authentication;

[TestFixture]
public class LoginActionTests
{
    private static LoginAction CreateAction(FakeSocketTransport transport)
    {
        return new LoginAction(() => transport, () => SessionFactory.LoginKey)
        {
            Host = "login.example",
            Port = 7171,
            AccountName = "account",
            Password = "secret",
            TimeoutMs = 15000
        };
    }

    [Test]
    public void Execute_WithValidCredentials_PopulatesSessionMotdAndCharacters()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(Frames.LoginMotdPayload("welcome back"), SessionFactory.LoginKey));
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(Frames.LoginCharacterListPayload("Rook"), SessionFactory.LoginKey));
        var action = CreateAction(transport);

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(action.LoginSession, Is.Not.Null);
            Assert.That(action.Motd, Is.EqualTo("welcome back"));
            Assert.That(action.Characters.Rows, Has.Count.EqualTo(1));
            Assert.That(action.Characters.Rows[0]["Name"], Is.EqualTo("Rook"));
        });
    }

    [Test]
    public void Execute_WhenServerRejects_ThrowsAuthenticationFailed()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(FrameCodec.EncodeEncrypted(Frames.LoginErrorPayload("bad credentials"), SessionFactory.LoginKey));
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
    public void Execute_WithNullHost_ThrowsInvalidArgument()
    {
        var action = CreateAction(new FakeSocketTransport());
        action.Host = null!;

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
    }

    [Test]
    public void Constructor_WithNullTransportFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginAction(null!, () => SessionFactory.LoginKey));
    }

    [Test]
    public void Constructor_WithNullKeyFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginAction(() => new FakeSocketTransport(), null!));
    }

    [Test]
    public void DefaultConstructor_WiresARealTransportFactory()
    {
        var action = new LoginAction();

        var field = typeof(LoginAction).GetField("_transportFactory", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var factory = (Func<ISocketTransport>)field.GetValue(action)!;

        using var transport = factory();

        Assert.That(transport, Is.InstanceOf<ISocketTransport>());
    }
}
