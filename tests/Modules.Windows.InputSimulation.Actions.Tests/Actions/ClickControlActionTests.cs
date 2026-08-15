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
public class ClickControlActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_WithoutCoordinates_ClicksTheControlCentre()
    {
        _test.WindowService.CenterX = 40;
        _test.WindowService.CenterY = 12;

        var action = new ClickControlAction(_test.Context) { Control = ActionHarness.Window(21) };
        action.Execute(new ActionContext());

        Assert.That(_test.InputSender.Clicks.Count, Is.EqualTo(1));
        var click = _test.InputSender.Clicks[0];
        Assert.That(click.Handle, Is.EqualTo(ActionHarness.Handle(21)));
        Assert.That(click.X, Is.EqualTo(40));
        Assert.That(click.Y, Is.EqualTo(12));
        Assert.That(click.Button, Is.EqualTo(MouseButton.Left));
        Assert.That(click.IsDoubleClick, Is.False);
    }

    [Test]
    public void Execute_WithCoordinates_PrefersTheSuppliedPosition()
    {
        _test.WindowService.CenterX = 40;
        _test.WindowService.CenterY = 12;

        var action = new ClickControlAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Button = MouseButton.Right,
            X = 5,
            Y = 6
        };
        action.Execute(new ActionContext());

        var click = _test.InputSender.Clicks[0];
        Assert.That(click.X, Is.EqualTo(5));
        Assert.That(click.Y, Is.EqualTo(6));
        Assert.That(click.Button, Is.EqualTo(MouseButton.Right));
    }

    [Test]
    public void Execute_WithOnlyXSupplied_KeepsCentreForY()
    {
        _test.WindowService.CenterY = 99;

        var action = new ClickControlAction(_test.Context) { Control = ActionHarness.Window(), X = 3 };
        action.Execute(new ActionContext());

        var click = _test.InputSender.Clicks[0];
        Assert.That(click.X, Is.EqualTo(3));
        Assert.That(click.Y, Is.EqualTo(99));
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new ClickControlAction(_test.Context) { Control = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new ClickControlAction();

        Assert.That(action.Button, Is.EqualTo(MouseButton.Left));
    }
}
