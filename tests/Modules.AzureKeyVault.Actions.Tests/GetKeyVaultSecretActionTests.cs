// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.AzureKeyVault.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.AzureKeyVault.Actions.Tests;

[TestFixture]
public class GetKeyVaultSecretActionTests
{
    [SetUp]
    public void SetUp() => provider = new FakeKeyVaultSecretProvider();

    private FakeKeyVaultSecretProvider provider = null!;

    [Test]
    public void Execute_WithValidInputs_ReturnsSecretValue()
    {
        var action = new GetKeyVaultSecretAction(provider) { VaultUrl = "https://vault.vault.azure.net/", SecretName = "name" };

        action.Execute(new ActionContext());

        Assert.That(action.SecretValue, Is.EqualTo("secret-value"));
        Assert.That(provider.VaultUri, Is.EqualTo(new Uri("https://vault.vault.azure.net/")));
        Assert.That(provider.SecretName, Is.EqualTo("name"));
    }

    [Test]
    public void Execute_WithBlankVaultUrl_ThrowsUnknownError()
    {
        var action = new GetKeyVaultSecretAction(provider) { VaultUrl = " ", SecretName = "name" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WithInvalidVaultUrl_ThrowsUnknownError()
    {
        var action = new GetKeyVaultSecretAction(provider) { VaultUrl = "not a url", SecretName = "name" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WithBlankSecretName_ThrowsUnknownError()
    {
        var action = new GetKeyVaultSecretAction(provider) { VaultUrl = "https://vault.vault.azure.net/", SecretName = "" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WhenProviderFails_ThrowsUnknownError()
    {
        provider.Exception = new InvalidOperationException("boom");
        var action = new GetKeyVaultSecretAction(provider) { VaultUrl = "https://vault.vault.azure.net/", SecretName = "name" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Constructor_WithNullProvider_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GetKeyVaultSecretAction(null!));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new GetKeyVaultSecretAction();

        Assert.That(action.SecretValue, Is.Null);
    }
}