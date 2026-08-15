// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Actions;

[TestFixture]
public class GetRegistryValueActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_WithExpandableKindAndExpandEnabled_ExpandsValue()
    {
        _test.Key.Values["Value"] = ("%TEMP%", RegistryValueKind.ExpandString);
        var action = new GetRegistryValueAction(_test.Context) { Path = "HIVE\\Path", Name = "Value", DefaultValue = "default", ExpandEnvironmentVariables = true };

        action.Execute(new ActionContext());

        Assert.That(action.Value, Is.EqualTo("%TEMP%"));
        Assert.That(_test.Key.ValueReads, Is.EqualTo(new[] { ("Value", (object?)"default", true) }));
    }

    [Test]
    public void Execute_WithExpandableKindAndExpandDisabled_DoesNotExpandValue()
    {
        _test.Key.Values["Value"] = ("%TEMP%", RegistryValueKind.ExpandString);
        var action = new GetRegistryValueAction(_test.Context) { Path = "HIVE\\Path", Name = "Value", DefaultValue = "default" };

        action.Execute(new ActionContext());

        Assert.That(_test.Key.ValueReads, Is.EqualTo(new[] { ("Value", (object?)"default", false) }));
    }

    [Test]
    public void Execute_WithPlainKindAndExpandEnabled_DoesNotExpandValue()
    {
        _test.Key.Values["Value"] = ("text", RegistryValueKind.String);
        var action = new GetRegistryValueAction(_test.Context) { Path = "HIVE\\Path", Name = "Value", ExpandEnvironmentVariables = true };

        action.Execute(new ActionContext());

        Assert.That(_test.Key.ValueReads, Is.EqualTo(new[] { ("Value", (object?)null, false) }));
    }

    [Test]
    public void Execute_WhenReadFails_ThrowsUnknownError()
    {
        _test.Key.ExceptionToThrow = new System.InvalidOperationException("boom");
        var action = new GetRegistryValueAction(_test.Context) { Path = "HIVE\\Path", Name = "Value" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new GetRegistryValueAction();

        Assert.That(action.ExpandEnvironmentVariables, Is.False);
    }
}
