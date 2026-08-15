// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Actions;

[TestFixture]
public class SetRegistryValueActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_WithString_SetsStringValue()
    {
        var action = NewAction(RegistryValueKind.String);
        action.String = "text";

        action.Execute(new ActionContext());

        AssertSet("text", RegistryValueKind.String);
    }

    [Test]
    public void Execute_WithExpandString_SetsExpandableValue()
    {
        var action = NewAction(RegistryValueKind.ExpandString);
        action.ExpandString = "%TEMP%";

        action.Execute(new ActionContext());

        AssertSet("%TEMP%", RegistryValueKind.ExpandString);
    }

    [Test]
    public void Execute_WithBinary_SetsBinaryValue()
    {
        var bytes = new byte[] { 1, 2 };
        var action = NewAction(RegistryValueKind.Binary);
        action.Binary = bytes;

        action.Execute(new ActionContext());

        AssertSet(bytes, RegistryValueKind.Binary);
    }

    [Test]
    public void Execute_WithDWord_SetsDWordValue()
    {
        var action = NewAction(RegistryValueKind.DWord);
        action.DWord = 7;

        action.Execute(new ActionContext());

        AssertSet(7, RegistryValueKind.DWord);
    }

    [Test]
    public void Execute_WithMultiString_SetsStringArrayValue()
    {
        var action = NewAction(RegistryValueKind.MultiString);
        action.MultiString = new List<string> { "a", "b" };

        action.Execute(new ActionContext());

        Assert.That(_test.Key.SetValues[0].Value, Is.EqualTo(new[] { "a", "b" }));
        Assert.That(_test.Key.SetValues[0].Kind, Is.EqualTo(RegistryValueKind.MultiString));
    }

    [Test]
    public void Execute_WithQWord_SetsQWordValue()
    {
        var action = NewAction(RegistryValueKind.QWord);
        action.QWord = 9;

        action.Execute(new ActionContext());

        AssertSet(9L, RegistryValueKind.QWord);
    }

    [Test]
    public void Execute_WithUnknownKind_ThrowsUnknownError()
    {
        var action = NewAction((RegistryValueKind)99);

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Execute_WhenSetFails_ThrowsUnknownError()
    {
        _test.Key.ExceptionToThrow = new System.InvalidOperationException("boom");
        var action = NewAction(RegistryValueKind.String);
        action.String = "text";

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Selectors_AreConstructible()
    {
        Assert.That(new SetStringRegistryValueActionSelector(), Is.Not.Null);
        Assert.That(new SetExpandStringRegistryValueActionSelector(), Is.Not.Null);
        Assert.That(new SetMultiStringRegistryValueActionSelector(), Is.Not.Null);
        Assert.That(new SetBinaryRegistryValueActionSelector(), Is.Not.Null);
        Assert.That(new SetInt32RegistryValueActionSelector(), Is.Not.Null);
        Assert.That(new SetInt64RegistryValueActionSelector(), Is.Not.Null);
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new SetRegistryValueAction();

        Assert.That(action.Kind, Is.EqualTo(RegistryValueKind.String));
    }

    private void AssertSet(object expectedValue, RegistryValueKind expectedKind)
    {
        Assert.That(_test.Key.SetValues.Count, Is.EqualTo(1));
        Assert.That(_test.Key.SetValues[0].Name, Is.EqualTo("Value"));
        Assert.That(_test.Key.SetValues[0].Value, Is.EqualTo(expectedValue));
        Assert.That(_test.Key.SetValues[0].Kind, Is.EqualTo(expectedKind));
    }

    private SetRegistryValueAction NewAction(RegistryValueKind kind)
    {
        return new SetRegistryValueAction(_test.Context) { Path = "HIVE\\Path", Name = "Value", Kind = kind };
    }
}
