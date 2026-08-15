// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NativeKind = Microsoft.Win32.RegistryValueKind;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests;

[TestFixture]
public class InfrastructureTests
{
    [Test]
    public void Context_RequiresRegistryService()
    {
        Assert.Throws<ArgumentNullException>(() => new RegistryContext(null!));
    }

    [Test]
    public void CreateDefault_WiresRegistryService()
    {
        Assert.That(RegistryContext.CreateDefault().RegistryService, Is.Not.Null);
    }

    [Test]
    public void Action_WithoutAContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new CreateRegistryKeyAction(null!));
    }

    [TestCase(RegistryValueKind.String, NativeKind.String)]
    [TestCase(RegistryValueKind.ExpandString, NativeKind.ExpandString)]
    [TestCase(RegistryValueKind.Binary, NativeKind.Binary)]
    [TestCase(RegistryValueKind.DWord, NativeKind.DWord)]
    [TestCase(RegistryValueKind.MultiString, NativeKind.MultiString)]
    [TestCase(RegistryValueKind.QWord, NativeKind.QWord)]
    public void ToNative_MapsEveryKind(RegistryValueKind value, NativeKind expected)
    {
        Assert.That(value.ToNative(), Is.EqualTo(expected));
    }

    [Test]
    public void ToNative_WithUnknownKind_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ((RegistryValueKind)99).ToNative());
    }

    [Test]
    public void CanExpandEnvironmentVariables_OnlyAllowsExpandString()
    {
        Assert.That(RegistryValueKind.ExpandString.CanExpandEnvironmentVariables(), Is.True);
        Assert.That(RegistryValueKind.String.CanExpandEnvironmentVariables(), Is.False);
        Assert.That(((RegistryValueKind)99).CanExpandEnvironmentVariables(), Is.False);
    }

    [Test]
    public void RegistryValueKind_ContainsExpectedValues()
    {
        Assert.That(Enum.GetNames(typeof(RegistryValueKind)), Is.EqualTo(new[] { "String", "ExpandString", "MultiString", "DWord", "QWord", "Binary" }));
    }

    [Test]
    public void ParseHive_ReturnsMatchingHiveIgnoringCase()
    {
        var hives = new[] { new FakeRegistryKey("First"), new FakeRegistryKey("HIVE") };

        Assert.That(RegistryExtensions.ParseHive("hive", hives), Is.SameAs(hives[1]));
    }

    [Test]
    public void ParseHive_WithUnknownHive_Throws()
    {
        var hives = new[] { new FakeRegistryKey("HIVE") };

        Assert.Throws<InvalidOperationException>(() => RegistryExtensions.ParseHive("Missing", hives));
    }

    [Test]
    public void ParseKey_OpensFirstSubKeyFromHive()
    {
        var hive = new FakeRegistryKey("HIVE");
        var child = new FakeRegistryKey("Child");
        hive.SubKeys["Child"] = child;

        var result = RegistryExtensions.ParseKey("HIVE\\Child", true, new[] { hive });

        Assert.That(result, Is.SameAs(child));
        Assert.That(hive.OpenedSubKeys, Is.EqualTo(new[] { ("Child", true) }));
    }

    [Test]
    public void ParseKey_OpensNestedKeysAndDisposesPrevious()
    {
        var hive = new FakeRegistryKey("HIVE");
        var first = new FakeRegistryKey("First");
        var second = new FakeRegistryKey("Second");
        hive.SubKeys["First"] = first;
        first.SubKeys["Second"] = second;

        var result = RegistryExtensions.ParseKey("HIVE\\First\\Second", false, new[] { hive });

        Assert.That(result, Is.SameAs(second));
        Assert.That(first.OpenedSubKeys, Is.EqualTo(new[] { ("Second", false) }));
        Assert.That(first.DisposeCount, Is.EqualTo(1));
    }

    [Test]
    public void ParseKey_WhenIntermediateIsNull_ContinuesFromHive()
    {
        var hive = new FakeRegistryKey("HIVE");
        var fallback = new FakeRegistryKey("Fallback");
        hive.SubKeys["Missing"] = null;
        hive.SubKeys["Fallback"] = fallback;

        var result = RegistryExtensions.ParseKey("HIVE\\Missing\\Fallback", true, new[] { hive });

        Assert.That(result, Is.SameAs(fallback));
        Assert.That(hive.OpenedSubKeys.Select(x => x.Name), Is.EqualTo(new[] { "Missing", "Fallback" }));
    }

    [Test]
    public void ParseKey_WithHiveOnly_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => RegistryExtensions.ParseKey("HIVE", true, new[] { new FakeRegistryKey("HIVE") }));
    }

    [Test]
    public void Execute_WhenOpenFails_ThrowsUnknownError()
    {
        var harness = new ActionHarness();
        harness.RegistryService.ExceptionToThrow = new InvalidOperationException("boom");
        var action = new CreateRegistryKeyAction(harness.Context) { Path = "HIVE\\Path", Name = "Child" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }
}
