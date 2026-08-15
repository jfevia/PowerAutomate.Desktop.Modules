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
public class DoubleClickControlActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_RequestsADoubleClickAtTheCentre()
    {
        _test.WindowService.CenterX = 8;
        _test.WindowService.CenterY = 9;

        var action = new DoubleClickControlAction(_test.Context) { Control = ActionHarness.Window(4) };
        action.Execute(new ActionContext());

        var click = _test.InputSender.Clicks[0];
        Assert.That(click.IsDoubleClick, Is.True);
        Assert.That(click.X, Is.EqualTo(8));
        Assert.That(click.Y, Is.EqualTo(9));
    }

    [Test]
    public void Execute_WithCoordinatesAndButton_PassesThemThrough()
    {
        var action = new DoubleClickControlAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Button = MouseButton.Middle,
            X = 1,
            Y = 2
        };
        action.Execute(new ActionContext());

        var click = _test.InputSender.Clicks[0];
        Assert.That(click.Button, Is.EqualTo(MouseButton.Middle));
        Assert.That(click.X, Is.EqualTo(1));
        Assert.That(click.Y, Is.EqualTo(2));
    }

    [Test]
    public void Execute_WithOnlyYSupplied_KeepsCentreForX()
    {
        _test.WindowService.CenterX = 77;

        var action = new DoubleClickControlAction(_test.Context) { Control = ActionHarness.Window(), Y = 4 };
        action.Execute(new ActionContext());

        var click = _test.InputSender.Clicks[0];
        Assert.That(click.X, Is.EqualTo(77));
        Assert.That(click.Y, Is.EqualTo(4));
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new DoubleClickControlAction(_test.Context) { Control = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new DoubleClickControlAction();

        Assert.That(action.Button, Is.EqualTo(MouseButton.Left));
    }
}
