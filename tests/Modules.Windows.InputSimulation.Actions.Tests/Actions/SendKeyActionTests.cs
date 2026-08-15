// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Actions;

[TestFixture]
public class SendKeyActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ForwardsTheRequestedKey()
    {
        var action = new SendKeyAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Key = VirtualKey.Tab
        };
        action.Execute(new ActionContext());

        Assert.That(_test.InputSender.Keys, Is.EqualTo(new[] { VirtualKey.Tab }));
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new SendKeyAction(_test.Context) { Control = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new SendKeyAction();

        Assert.That(action.Key, Is.EqualTo(VirtualKey.Enter));
    }
}
