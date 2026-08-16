// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests;

[TestFixture]
public class ActionErrorsTests
{
    [Test]
    public void Create_AppendsErrorSuffixToTheCode()
    {
        var exception = ActionErrors.Create(ErrorCodes.NotConnected, "no session");

        Assert.Multiple(() =>
        {
            Assert.That(exception.Name, Is.EqualTo("NotConnectedError"));
            Assert.That(exception.Message, Is.EqualTo("no session"));
        });
    }

    [Test]
    public void Create_WithInnerException_KeepsIt()
    {
        var inner = new InvalidOperationException("cause");

        var exception = ActionErrors.Create(ErrorCodes.Unknown, "wrapped", inner);

        Assert.That(exception.InnerException, Is.SameAs(inner));
    }

    [Test]
    public void Translate_WithActionException_ReturnsItUnchanged()
    {
        var original = ActionErrors.Create(ErrorCodes.Timeout, "slow");

        Assert.That(ActionErrors.Translate(original), Is.SameAs(original));
    }

    [Test]
    public void Translate_WithProtocolException_MapsToProtocolError()
    {
        var exception = ActionErrors.Translate(new ProtocolException("bad frame"));

        Assert.That(exception.Name, Is.EqualTo("ProtocolErrorError"));
    }

    [Test]
    public void Translate_WithTimeoutException_MapsToTimeout()
    {
        Assert.That(ActionErrors.Translate(new TimeoutException("late")).Name, Is.EqualTo("TimeoutError"));
    }

    [Test]
    public void Translate_WithArgumentException_MapsToInvalidArgument()
    {
        Assert.That(
            ActionErrors.Translate(new ArgumentException("bad")).Name,
            Is.EqualTo("InvalidArgumentError"));
    }

    [Test]
    public void Translate_WithUnexpectedException_MapsToUnknown()
    {
        Assert.That(
            ActionErrors.Translate(new InvalidOperationException("boom")).Name,
            Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Translate_WithNullException_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ActionErrors.Translate(null!));
    }
}

[TestFixture]
public class OpenTibiaActionBaseTests
{
    [Test]
    public void Execute_OnSuccess_RunsTheAction()
    {
        var action = new ProbeAction();

        action.Execute(new ActionContext());

        Assert.That(action.Ran, Is.True);
    }

    [Test]
    public void Execute_WhenRunThrows_TranslatesToActionException()
    {
        var action = new ProbeAction { Failure = new ProtocolException("bad frame") };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("ProtocolErrorError"));
    }

    private sealed class ProbeAction : OpenTibiaActionBase
    {
        public bool Ran { get; private set; }

        public Exception? Failure { get; set; }

        protected override void Run(ActionContext context)
        {
            if (Failure != null)
            {
                throw Failure;
            }

            Ran = true;
        }
    }
}
