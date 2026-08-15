// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Actions;

[TestFixture]
public class DeleteRegistryValueActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [TestCase(false)]
    [TestCase(true)]
    public void Execute_DeletesValue(bool throwOnMissing)
    {
        var action = new DeleteRegistryValueAction(_test.Context) { Path = "HIVE\\Path", Name = "Value", ThrowOnMissingValue = throwOnMissing };

        action.Execute(new ActionContext());

        Assert.That(_test.Key.DeletedValues, Is.EqualTo(new[] { ("Value", throwOnMissing) }));
    }

    [Test]
    public void Execute_WhenDeleteFails_ThrowsUnknownError()
    {
        _test.Key.ExceptionToThrow = new System.InvalidOperationException("boom");
        var action = new DeleteRegistryValueAction(_test.Context) { Path = "HIVE\\Path", Name = "Value" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new DeleteRegistryValueAction();

        Assert.That(action.ThrowOnMissingValue, Is.False);
    }
}
