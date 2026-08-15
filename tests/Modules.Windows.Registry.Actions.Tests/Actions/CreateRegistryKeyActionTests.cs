// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Actions;

[TestFixture]
public class CreateRegistryKeyActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_CreatesSubKey()
    {
        var action = new CreateRegistryKeyAction(_test.Context) { Path = "HIVE\\Path", Name = "Child" };

        action.Execute(new ActionContext());

        Assert.That(_test.RegistryService.OpenedKeys, Is.EqualTo(new[] { ("HIVE\\Path", true) }));
        Assert.That(_test.Key.SubKeys.Keys, Does.Contain("Child"));
        Assert.That(_test.Key.DisposeCount, Is.EqualTo(1));
    }

    [Test]
    public void Execute_WhenCreateFails_ThrowsUnknownError()
    {
        _test.Key.ExceptionToThrow = new System.InvalidOperationException("boom");
        var action = new CreateRegistryKeyAction(_test.Context) { Path = "HIVE\\Path", Name = "Child" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new CreateRegistryKeyAction();

        Assert.That(action.Name, Is.Null);
    }
}
