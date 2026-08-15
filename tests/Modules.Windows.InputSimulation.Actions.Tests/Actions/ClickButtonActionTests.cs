// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Actions;

[TestFixture]
public class ClickButtonActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_SendsButtonClickToTheControl()
    {
        var action = new ClickButtonAction(_test.Context) { Control = ActionHarness.Window(31) };
        action.Execute(new ActionContext());

        Assert.That(_test.MessageDispatcher.Sends.Count, Is.EqualTo(1));
        var sent = _test.MessageDispatcher.Sends[0];
        Assert.That(sent.Handle, Is.EqualTo(ActionHarness.Handle(31)));
        Assert.That(sent.Message, Is.EqualTo(0x00F5));
    }

    [Test]
    public void Execute_WhenDeliveryDenied_ThrowsAccessDeniedError()
    {
        _test.MessageDispatcher.SendException = new AccessDeniedException();

        var action = new ClickButtonAction(_test.Context) { Control = ActionHarness.Window() };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.AccessDenied));
    }

    [Test]
    public void Execute_WhenDeliveryFails_ThrowsMessageDeliveryError()
    {
        _test.MessageDispatcher.SendException = new MessageDeliveryException("gone");

        var action = new ClickButtonAction(_test.Context) { Control = ActionHarness.Window() };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.MessageDelivery));
    }

    [Test]
    public void Execute_WithoutControl_ThrowsUnknownError()
    {
        var action = new ClickButtonAction(_test.Context) { Control = null! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new ClickButtonAction();

        Assert.That(action.Control, Is.Null);
    }
}
