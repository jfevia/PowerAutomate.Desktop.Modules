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
public class SelectListBoxItemActionTests
{
    private const uint FindStringExact = 0x01A2;
    private const uint SetCurrentSelection = 0x0186;

    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ByIndex_SelectsAndNotifies()
    {
        _test.WindowService.ParentResult = ActionHarness.Handle(700);
        _test.MessageDispatcher.SendResults[SetCurrentSelection] = (IntPtr)1;

        var action = new SelectListBoxItemAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Index = 1
        };
        action.Execute(new ActionContext());

        Assert.That(action.SelectedIndex, Is.EqualTo(1));
        Assert.That(_test.MessageDispatcher.Sends[0].Message, Is.EqualTo(SetCurrentSelection));
        Assert.That(_test.MessageDispatcher.Sends[1].Message, Is.EqualTo(0x0111));
    }

    [Test]
    public void Execute_ByText_ResolvesTheIndexFirst()
    {
        _test.MessageDispatcher.SendResults[FindStringExact] = (IntPtr)5;
        _test.MessageDispatcher.SendResults[SetCurrentSelection] = (IntPtr)5;

        var action = new SelectListBoxItemAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Text = "Item"
        };
        action.Execute(new ActionContext());

        Assert.That(action.SelectedIndex, Is.EqualTo(5));
        Assert.That(_test.MessageDispatcher.Sends[0].Text, Is.EqualTo("Item"));
    }

    [Test]
    public void Execute_WhenTextNotFound_ThrowsControlNotFoundError()
    {
        _test.MessageDispatcher.SendResults[FindStringExact] = (IntPtr)(-1);

        var action = new SelectListBoxItemAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Text = "Nope"
        };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.ControlNotFound));
    }

    [Test]
    public void Execute_WhenIndexRejected_ThrowsControlNotFoundError()
    {
        _test.MessageDispatcher.SendResults[SetCurrentSelection] = (IntPtr)(-1);

        var action = new SelectListBoxItemAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Index = 42
        };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.ControlNotFound));
    }

    [Test]
    public void Execute_WithoutTextOrIndex_ThrowsUnknownError()
    {
        var action = new SelectListBoxItemAction(_test.Context) { Control = ActionHarness.Window() };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new SelectListBoxItemAction(_test.Context) { Control = null!, Index = 0 };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new SelectListBoxItemAction();

        Assert.That(action.SelectedIndex, Is.Zero);
    }
}
