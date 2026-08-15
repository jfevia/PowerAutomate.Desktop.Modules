// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Actions;

[TestFixture]
public class GetSubKeysInRegistryKeyActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ReturnsSubKeyNames()
    {
        _test.Key.SubKeys["A"] = new FakeRegistryKey("A");
        _test.Key.SubKeys["B"] = new FakeRegistryKey("B");
        var action = new GetSubKeysInRegistryKeyAction(_test.Context) { Path = "HIVE\\Path" };

        action.Execute(new ActionContext());

        Assert.That(action.SubKeys, Is.EquivalentTo(new[] { "A", "B" }));
    }

    [Test]
    public void Execute_WhenReadFails_ThrowsUnknownError()
    {
        _test.Key.ExceptionToThrow = new System.InvalidOperationException("boom");
        var action = new GetSubKeysInRegistryKeyAction(_test.Context) { Path = "HIVE\\Path" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new GetSubKeysInRegistryKeyAction();

        Assert.That(action.Path, Is.Null);
    }
}
