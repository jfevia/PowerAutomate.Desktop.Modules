// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Identity.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Identity.Actions.Tests;

[TestFixture]
public class GetAccessTokenActionTests
{
    [SetUp]
    public void SetUp() => provider = new FakeAccessTokenProvider();

    private FakeAccessTokenProvider provider = null!;

    [Test]
    public void Execute_WithValidInputs_ReturnsToken()
    {
        var action = CreateAction();

        action.Execute(new ActionContext());

        Assert.That(action.Token, Is.EqualTo("token"));
        Assert.That(provider.Authority, Is.EqualTo(new Uri("https://login.microsoftonline.com/tenant")));
        Assert.That(provider.ClientId, Is.EqualTo("client"));
        Assert.That(provider.ClientSecret, Is.EqualTo("secret"));
        Assert.That(provider.Scopes, Is.EquivalentTo(new[] { "scope/.default" }));
    }

    [Test]
    public void Execute_WithBlankClientId_ThrowsUnknownError()
    {
        var action = CreateAction();
        action.ClientId = " ";

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WithBlankClientSecret_ThrowsUnknownError()
    {
        var action = CreateAction();
        action.ClientSecret = "";

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WithInvalidAuthority_ThrowsUnknownError()
    {
        var action = CreateAction();
        action.Authority = "relative";

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WithNullScopes_ThrowsUnknownError()
    {
        var action = CreateAction();
        action.Scopes = null!;

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WithEmptyScopes_ThrowsUnknownError()
    {
        var action = CreateAction();
        action.Scopes = new List<string>();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WithBlankScope_ThrowsUnknownError()
    {
        var action = CreateAction();
        action.Scopes = new List<string> { "scope", " " };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WhenProviderFails_ThrowsUnknownError()
    {
        provider.Exception = new InvalidOperationException("boom");
        var action = CreateAction();

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Constructor_WithNullProvider_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GetAccessTokenAction(null!));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new GetAccessTokenAction();

        Assert.That(action.Token, Is.Null);
    }

    private GetAccessTokenAction CreateAction() => new(provider)
    {
        Authority = "https://login.microsoftonline.com/tenant",
        ClientId = "client",
        ClientSecret = "secret",
        Scopes = new List<string> { "scope/.default" }
    };
}