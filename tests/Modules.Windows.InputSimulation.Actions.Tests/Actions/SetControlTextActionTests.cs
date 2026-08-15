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
public class SetControlTextActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_SendsTheTextInOneMessage()
    {
        var action = new SetControlTextAction(_test.Context)
        {
            Control = ActionHarness.Window(6),
            Text = "hello"
        };
        action.Execute(new ActionContext());

        Assert.That(_test.MessageDispatcher.Sends.Count, Is.EqualTo(1));
        Assert.That(_test.MessageDispatcher.Sends[0].Message, Is.EqualTo(0x000C));
        Assert.That(_test.MessageDispatcher.Sends[0].Text, Is.EqualTo("hello"));
    }

    [Test]
    public void Execute_WithNullText_SendsAnEmptyString()
    {
        var action = new SetControlTextAction(_test.Context)
        {
            Control = ActionHarness.Window(),
            Text = null!
        };
        action.Execute(new ActionContext());

        Assert.That(_test.MessageDispatcher.Sends[0].Text, Is.Empty);
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new SetControlTextAction(_test.Context) { Control = null!, Text = "x" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new SetControlTextAction();

        Assert.That(action.Text, Is.Null);
    }
}
