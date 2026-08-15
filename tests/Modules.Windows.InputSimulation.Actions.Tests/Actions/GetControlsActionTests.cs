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
public class GetControlsActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ReturnsOneEntryPerChildWindow()
    {
        _test.WindowService.Children = new[] { ActionHarness.Handle(1), ActionHarness.Handle(2) };

        var action = new GetControlsAction(_test.Context) { Window = ActionHarness.Window(5) };
        action.Execute(new ActionContext());

        Assert.That(action.Controls.Count, Is.EqualTo(2));
        Assert.That(action.Controls[0].Handle, Is.EqualTo(1));
        Assert.That(action.Controls[1].Handle, Is.EqualTo(2));
        Assert.That(_test.WindowService.RequestedRecursive, Is.True);
    }

    [Test]
    public void Execute_WhenNoChildren_ReturnsEmptyList()
    {
        _test.WindowService.Children = Array.Empty<IntPtr>();

        var action = new GetControlsAction(_test.Context) { Window = ActionHarness.Window(), Recursive = false };
        action.Execute(new ActionContext());

        Assert.That(action.Controls, Is.Empty);
        Assert.That(_test.WindowService.RequestedRecursive, Is.False);
    }

    [Test]
    public void Execute_WithoutWindow_ThrowsUnknownError()
    {
        var action = new GetControlsAction(_test.Context) { Window = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new GetControlsAction();

        Assert.That(action.Recursive, Is.True);
    }
}
