// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Actions;

[TestFixture]
public class GetControlTextActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ReturnsTheControlText()
    {
        _test.WindowService.TextResult = "contents";

        var action = new GetControlTextAction(_test.Context) { Control = ActionHarness.Window() };
        action.Execute(new ActionContext());

        Assert.That(action.Text, Is.EqualTo("contents"));
    }

    [Test]
    public void Execute_WhenControlHasNoText_ReturnsEmpty()
    {
        _test.WindowService.TextResult = string.Empty;

        var action = new GetControlTextAction(_test.Context) { Control = ActionHarness.Window() };
        action.Execute(new ActionContext());

        Assert.That(action.Text, Is.Empty);
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new GetControlTextAction(_test.Context) { Control = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new GetControlTextAction();

        Assert.That(action.Text, Is.Null);
    }
}
