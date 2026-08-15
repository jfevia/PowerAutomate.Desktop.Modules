// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Actions;

[TestFixture]
public class SetCheckBoxStateActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_SetsStateAndNotifiesTheParent()
    {
        _test.WindowService.ParentResult = ActionHarness.Handle(500);
        _test.WindowService.ControlIdResult = 9;

        var action = new SetCheckBoxStateAction(_test.Context)
        {
            Control = ActionHarness.Window(12),
            State = CheckBoxState.Checked
        };
        action.Execute(new ActionContext());

        Assert.That(_test.MessageDispatcher.Sends.Count, Is.EqualTo(2));
        Assert.That(_test.MessageDispatcher.Sends[0].Message, Is.EqualTo(0x00F1));
        Assert.That(_test.MessageDispatcher.Sends[0].WParam, Is.EqualTo((IntPtr)1));
        Assert.That(_test.MessageDispatcher.Sends[1].Message, Is.EqualTo(0x0111));
        Assert.That(_test.MessageDispatcher.Sends[1].Handle, Is.EqualTo(ActionHarness.Handle(500)));
    }

    [Test]
    public void Execute_WithoutParent_SkipsTheNotification()
    {
        _test.WindowService.ParentResult = IntPtr.Zero;

        var action = new SetCheckBoxStateAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            State = CheckBoxState.Unchecked
        };
        action.Execute(new ActionContext());

        Assert.That(_test.MessageDispatcher.Sends.Count, Is.EqualTo(1));
        Assert.That(_test.MessageDispatcher.Sends[0].WParam, Is.EqualTo(IntPtr.Zero));
    }

    [Test]
    public void Execute_WithIndeterminateState_SendsTheThirdState()
    {
        var action = new SetCheckBoxStateAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            State = CheckBoxState.Indeterminate
        };
        action.Execute(new ActionContext());

        Assert.That(_test.MessageDispatcher.Sends[0].WParam, Is.EqualTo((IntPtr)2));
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new SetCheckBoxStateAction(_test.Context) { Control = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new SetCheckBoxStateAction();

        Assert.That(action.State, Is.EqualTo(CheckBoxState.Checked));
    }
}
