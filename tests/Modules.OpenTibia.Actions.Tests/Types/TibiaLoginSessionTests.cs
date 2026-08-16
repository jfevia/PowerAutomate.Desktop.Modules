// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Types;

[TestFixture]
public class TibiaLoginSessionTests
{
    private static TibiaLoginSession CreateSession()
    {
        return SessionFactory.CreateLoginSession(new FakeSocketTransport(), "Rook", "Knight");
    }

    private static LoginResult MakeResult()
    {
        return new LoginResult(Array.Empty<CharacterListEntry>(), "m", 0);
    }

    [Test]
    public void Constructor_KeepsEveryVisibleProperty()
    {
        var session = CreateSession();

        Assert.Multiple(() =>
        {
            Assert.That(session.Host, Is.EqualTo("login.example"));
            Assert.That(session.Port, Is.EqualTo(7171));
            Assert.That(session.AccountName, Is.EqualTo("account"));
            Assert.That(session.Motd, Is.EqualTo("welcome"));
            Assert.That(session.PremiumDays, Is.EqualTo(42));
        });
    }

    [Test]
    public void Transport_AndClient_AndPassword_AndResult_AreAccessibleForActions()
    {
        var transport = new FakeSocketTransport();
        var session = SessionFactory.CreateLoginSession(transport, "Rook");

        Assert.Multiple(() =>
        {
            Assert.That(session.Transport, Is.SameAs(transport));
            Assert.That(session.Client, Is.Not.Null);
            Assert.That(session.Password, Is.EqualTo("secret"));
            Assert.That(session.Result.Characters, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void ToString_DescribesTheSession()
    {
        Assert.That(CreateSession().ToString(), Is.EqualTo("account @ login.example:7171"));
    }

    [Test]
    public void DoesNotImplementICloneable()
    {
        Assert.That(typeof(ICloneable).IsAssignableFrom(typeof(TibiaLoginSession)), Is.False);
    }

    [Test]
    public void Constructor_WithNullTransport_Throws()
    {
        var client = new TibiaLoginClient(new FakeSocketTransport());

        Assert.Throws<ArgumentNullException>(() => new TibiaLoginSession(
            null!, client, "h", 1, "a", "p", MakeResult()));
    }

    [Test]
    public void Constructor_WithNullClient_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TibiaLoginSession(
            new FakeSocketTransport(), null!, "h", 1, "a", "p", MakeResult()));
    }

    [Test]
    public void Constructor_WithNullHost_Throws()
    {
        var client = new TibiaLoginClient(new FakeSocketTransport());

        Assert.Throws<ArgumentNullException>(() => new TibiaLoginSession(
            new FakeSocketTransport(), client, null!, 1, "a", "p", MakeResult()));
    }

    [Test]
    public void Constructor_WithNullAccountName_Throws()
    {
        var client = new TibiaLoginClient(new FakeSocketTransport());

        Assert.Throws<ArgumentNullException>(() => new TibiaLoginSession(
            new FakeSocketTransport(), client, "h", 1, null!, "p", MakeResult()));
    }

    [Test]
    public void Constructor_WithNullPassword_Throws()
    {
        var client = new TibiaLoginClient(new FakeSocketTransport());

        Assert.Throws<ArgumentNullException>(() => new TibiaLoginSession(
            new FakeSocketTransport(), client, "h", 1, "a", null!, MakeResult()));
    }

    [Test]
    public void Constructor_WithNullResult_Throws()
    {
        var client = new TibiaLoginClient(new FakeSocketTransport());

        Assert.Throws<ArgumentNullException>(() => new TibiaLoginSession(
            new FakeSocketTransport(), client, "h", 1, "a", "p", null!));
    }
}
