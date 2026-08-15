// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Actions;

[TestFixture]
public class SelectComboBoxItemActionTests
{
    private const uint FindStringExact = 0x0158;
    private const uint SetCurrentSelection = 0x014E;

    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ByIndex_SelectsAndNotifies()
    {
        _test.WindowService.ParentResult = ActionHarness.Handle(900);
        _test.MessageDispatcher.SendResults[SetCurrentSelection] = (IntPtr)3;

        var action = new SelectComboBoxItemAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Index = 3
        };
        action.Execute(new ActionContext());

        Assert.That(action.SelectedIndex, Is.EqualTo(3));
        Assert.That(_test.MessageDispatcher.Sends[0].Message, Is.EqualTo(SetCurrentSelection));
        Assert.That(_test.MessageDispatcher.Sends[1].Message, Is.EqualTo(0x0111));
    }

    [Test]
    public void Execute_ByText_ResolvesTheIndexFirst()
    {
        _test.MessageDispatcher.SendResults[FindStringExact] = (IntPtr)2;
        _test.MessageDispatcher.SendResults[SetCurrentSelection] = (IntPtr)2;

        var action = new SelectComboBoxItemAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Text = "Second"
        };
        action.Execute(new ActionContext());

        Assert.That(action.SelectedIndex, Is.EqualTo(2));
        Assert.That(_test.MessageDispatcher.Sends[0].Message, Is.EqualTo(FindStringExact));
        Assert.That(_test.MessageDispatcher.Sends[0].Text, Is.EqualTo("Second"));
    }

    [Test]
    public void Execute_WhenTextNotFound_ThrowsControlNotFoundError()
    {
        _test.MessageDispatcher.SendResults[FindStringExact] = (IntPtr)(-1);

        var action = new SelectComboBoxItemAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Text = "Missing"
        };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.ControlNotFound));
    }

    [Test]
    public void Execute_WhenIndexRejected_ThrowsControlNotFoundError()
    {
        _test.MessageDispatcher.SendResults[SetCurrentSelection] = (IntPtr)(-1);

        var action = new SelectComboBoxItemAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Index = 99
        };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.ControlNotFound));
    }

    [Test]
    public void Execute_WithoutTextOrIndex_ThrowsUnknownError()
    {
        var action = new SelectComboBoxItemAction(_test.Context) { Control = ActionHarness.Window() };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new SelectComboBoxItemAction(_test.Context) { Control = null!, Index = 0 };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new SelectComboBoxItemAction();

        Assert.That(action.SelectedIndex, Is.Zero);
    }
}
