// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Extensions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests;

[TestFixture]
public class InfrastructureTests
{
    [Test]
    public void ToActionException_MapsAccessDenied()
    {
        Assert.That(new AccessDeniedException().ToActionException().Name, Is.EqualTo(ErrorCodes.AccessDenied));
    }

    [Test]
    public void ToActionException_MapsWindowNotFound()
    {
        var exception = new WindowNotFoundException("title 'x'").ToActionException();

        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.WindowNotFound));
    }

    [Test]
    public void ToActionException_MapsControlNotFound()
    {
        Assert.That(new ControlNotFoundException("id 4").ToActionException().Name, Is.EqualTo(ErrorCodes.ControlNotFound));
    }

    [Test]
    public void ToActionException_MapsMessageDelivery()
    {
        Assert.That(new MessageDeliveryException("busy").ToActionException().Name, Is.EqualTo(ErrorCodes.MessageDelivery));
    }

    [Test]
    public void ToActionException_MapsAnythingElseToUnknown()
    {
        Assert.That(new InvalidOperationException("boom").ToActionException().Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void ToActionException_KeepsAnExistingActionException()
    {
        var original = new ActionException(ErrorCodes.Unknown, "kept");

        Assert.That(original.ToActionException(), Is.SameAs(original));
    }

    [Test]
    public void WindowNotFoundException_KeepsTheCriteria()
    {
        Assert.That(new WindowNotFoundException("title 'a'").Criteria, Is.EqualTo("title 'a'"));
    }

    [Test]
    public void ControlNotFoundException_KeepsTheCriteria()
    {
        Assert.That(new ControlNotFoundException("id 1").Criteria, Is.EqualTo("id 1"));
    }

    [Test]
    public void Context_RequiresEveryCollaborator()
    {
        var harness = new ActionHarness();

        Assert.Throws<ArgumentNullException>(() => new InputSimulationContext(null!, harness.InputSender, harness.MessageDispatcher));
        Assert.Throws<ArgumentNullException>(() => new InputSimulationContext(harness.WindowService, null!, harness.MessageDispatcher));
        Assert.Throws<ArgumentNullException>(() => new InputSimulationContext(harness.WindowService, harness.InputSender, null!));
    }

    [Test]
    public void CreateDefault_WiresEveryCollaborator()
    {
        var context = InputSimulationContext.CreateDefault();

        Assert.That(context.WindowService, Is.Not.Null);
        Assert.That(context.InputSender, Is.Not.Null);
        Assert.That(context.MessageDispatcher, Is.Not.Null);
    }

    [Test]
    public void Action_WithoutAContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions.ClickControlAction(null!));
    }

    [Test]
    public void WindowObject_DefaultConstructor_IsUsableBySerialization()
    {
        var window = new WindowObject();

        Assert.That(window.Handle, Is.Zero);
    }

    [Test]
    public void WindowObject_ToString_PrefersTheTitle()
    {
        Assert.That(new WindowObject(1, "Edit", "Name", 0, 0, true, true, 0, 0).ToString(), Is.EqualTo("Name (Edit)"));
    }

    [Test]
    public void WindowObject_ToString_FallsBackToTheClassAndHandle()
    {
        Assert.That(new WindowObject(7, "Edit", string.Empty, 0, 0, true, true, 0, 0).ToString(), Is.EqualTo("Edit (7)"));
    }

    [Test]
    public void WindowObject_NativeHandle_MirrorsTheHandle()
    {
        Assert.That(new WindowObject(9, "c", "t", 0, 0, true, true, 0, 0).NativeHandle, Is.EqualTo((IntPtr)9));
    }

    [Test]
    public void WindowObject_ComparesByHandle()
    {
        var first = new WindowObject(1, "c", "t", 0, 0, true, true, 0, 0);
        var second = new WindowObject(2, "c", "t", 0, 0, true, true, 0, 0);

        Assert.That(first.CompareTo(second), Is.LessThan(0));
        Assert.That(first.CompareTo(first), Is.Zero);
        Assert.That(first.CompareTo((WindowObject?)null), Is.GreaterThan(0));
    }

    [Test]
    public void WindowObject_ComparesAgainstBoxedValues()
    {
        var first = new WindowObject(1, "c", "t", 0, 0, true, true, 0, 0);
        object second = new WindowObject(2, "c", "t", 0, 0, true, true, 0, 0);

        Assert.That(first.CompareTo(second), Is.LessThan(0));
        Assert.That(first.CompareTo((object?)null), Is.GreaterThan(0));
        Assert.That(first.CompareTo((object)first), Is.Zero);
        Assert.Throws<ArgumentException>(() => first.CompareTo("not a window"));
    }

    [Test]
    public void SearchCriterion_KeepsWhatItWasGiven()
    {
        var criterion = new PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Services.SearchCriterion("title", "Main");

        Assert.That(criterion.Name, Is.EqualTo("title"));
        Assert.That(criterion.Value, Is.EqualTo("Main"));
    }
}
