// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Actions;

[TestFixture]
public class DeleteRegistryKeyActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [TestCase(false)]
    [TestCase(true)]
    public void Execute_DeletesSubKey(bool throwOnMissing)
    {
        var action = new DeleteRegistryKeyAction(_test.Context) { Path = "HIVE\\Path", Name = "Child", ThrowOnMissingSubKey = throwOnMissing };

        action.Execute(new ActionContext());

        Assert.That(_test.Key.DeletedSubKeys, Is.EqualTo(new[] { ("Child", throwOnMissing) }));
    }

    [Test]
    public void Execute_WhenDeleteFails_ThrowsUnknownError()
    {
        _test.Key.ExceptionToThrow = new System.InvalidOperationException("boom");
        var action = new DeleteRegistryKeyAction(_test.Context) { Path = "HIVE\\Path", Name = "Child" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new DeleteRegistryKeyAction();

        Assert.That(action.ThrowOnMissingSubKey, Is.False);
    }
}
