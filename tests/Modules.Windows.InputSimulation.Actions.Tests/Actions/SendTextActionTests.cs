// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Actions;

[TestFixture]
public class SendTextActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ForwardsTextAndDelay()
    {
        var action = new SendTextAction(_test.Context)
        {
            Control = ActionHarness.Window(14),
            Text = "abc",
            DelayMilliseconds = 25
        };
        action.Execute(new ActionContext());

        Assert.That(_test.InputSender.Texts.Count, Is.EqualTo(1));
        var call = _test.InputSender.Texts[0];
        Assert.That(call.Handle, Is.EqualTo(ActionHarness.Handle(14)));
        Assert.That(call.Text, Is.EqualTo("abc"));
        Assert.That(call.DelayMilliseconds, Is.EqualTo(25));
    }

    [Test]
    public void Execute_WithNullText_SendsAnEmptyString()
    {
        var action = new SendTextAction(_test.Context) { Control = ActionHarness.Window(), Text = null! };
        action.Execute(new ActionContext());

        Assert.That(_test.InputSender.Texts[0].Text, Is.Empty);
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new SendTextAction(_test.Context) { Control = null!, Text = "a" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new SendTextAction();

        Assert.That(action.DelayMilliseconds, Is.Zero);
    }
}
