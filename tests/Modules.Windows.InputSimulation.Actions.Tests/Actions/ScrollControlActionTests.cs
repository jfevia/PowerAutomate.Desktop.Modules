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
public class ScrollControlActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ScrollsAtTheScreenPositionOfTheCentre()
    {
        _test.WindowService.ScreenX = 300;
        _test.WindowService.ScreenY = 400;

        var action = new ScrollControlAction(_test.Context)
        {
            Control = ActionHarness.Window(2),
            Direction = ScrollDirection.Up,
            Notches = 4
        };
        action.Execute(new ActionContext());

        Assert.That(_test.InputSender.Scrolls.Count, Is.EqualTo(1));
        var scroll = _test.InputSender.Scrolls[0];
        Assert.That(scroll.Direction, Is.EqualTo(ScrollDirection.Up));
        Assert.That(scroll.Notches, Is.EqualTo(4));
        Assert.That(scroll.ScreenX, Is.EqualTo(300));
        Assert.That(scroll.ScreenY, Is.EqualTo(400));
    }

    [TestCase(0)]
    [TestCase(-2)]
    public void Execute_WithoutPositiveNotches_ThrowsUnknownError(int notches)
    {
        var action = new ScrollControlAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Notches = notches
        };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
        Assert.That(_test.InputSender.Scrolls, Is.Empty);
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new ScrollControlAction(_test.Context) { Control = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new ScrollControlAction();

        Assert.That(action.Direction, Is.EqualTo(ScrollDirection.Down));
        Assert.That(action.Notches, Is.EqualTo(3));
    }
}
