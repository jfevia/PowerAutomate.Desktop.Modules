// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Actions;

[TestFixture]
public class GetValuesInRegistryKeyActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ReturnsValueNames()
    {
        _test.Key.Values["A"] = (1, RegistryValueKind.DWord);
        _test.Key.Values["B"] = ("two", RegistryValueKind.String);
        var action = new GetValuesInRegistryKeyAction(_test.Context) { Path = "HIVE\\Path" };

        action.Execute(new ActionContext());

        Assert.That(action.Values, Is.EquivalentTo(new[] { "A", "B" }));
    }

    [Test]
    public void Execute_WhenReadFails_ThrowsUnknownError()
    {
        _test.Key.ExceptionToThrow = new System.InvalidOperationException("boom");
        var action = new GetValuesInRegistryKeyAction(_test.Context) { Path = "HIVE\\Path" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new GetValuesInRegistryKeyAction();

        Assert.That(action.Path, Is.Null);
    }
}
