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
public class FindControlActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_WhenControlFound_SetsControlOutput()
    {
        _test.WindowService.FoundControl = ActionHarness.Handle(77);
        _test.WindowService.ClassNameResult = "Edit";

        var action = new FindControlAction(_test.Context)
        {
            Window = ActionHarness.Window(10),
            Text = "Name",
            Recursive = false
        };
        action.Execute(new ActionContext());

        Assert.That(action.Control.Handle, Is.EqualTo(77));
        Assert.That(_test.WindowService.RequestedParent, Is.EqualTo(ActionHarness.Handle(10)));
        Assert.That(_test.WindowService.RequestedRecursive, Is.False);
    }

    [Test]
    public void Execute_WhenControlMissing_ThrowsControlNotFoundError()
    {
        _test.WindowService.FoundControl = IntPtr.Zero;

        var action = new FindControlAction(_test.Context) { Window = ActionHarness.Window() };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.ControlNotFound));
    }

    [Test]
    public void Execute_WithoutWindow_ThrowsUnknownError()
    {
        var action = new FindControlAction(_test.Context) { Window = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void Execute_WithEveryCriterion_StillResolvesControl()
    {
        _test.WindowService.FoundControl = ActionHarness.Handle(3);

        var action = new FindControlAction(_test.Context)
        {
            Window = ActionHarness.Window(),
            Text = "Text",
            ClassName = "Button",
            ControlId = 12,
            TimeoutMilliseconds = 10
        };
        action.Execute(new ActionContext());

        Assert.That(action.Control.Handle, Is.EqualTo(3));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new FindControlAction();

        Assert.That(action.Recursive, Is.True);
    }
}
