// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Tasks.Actions;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions.Tests;

[TestFixture]
public class TaskDelayActionTests
{
    [Test]
    public void Execute_CreatesTaskObject()
    {
        var runner = new FakeTaskRunner();
        var action = new TaskDelayAction(runner) { Name = "Delay", DelayInMilliseconds = 5 };

        action.Execute(new ActionContext());

        Assert.That(action.Task.Name, Is.EqualTo("Delay"));
        Assert.That(runner.DelayMilliseconds, Is.EqualTo(5));
    }

    [Test]
    public void Execute_WhenRunnerFails_WrapsActionException()
    {
        var action = new TaskDelayAction(new FakeTaskRunner { DelayException = new InvalidOperationException("Failed") });

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));

        Assert.That(exception!.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Constructor_WithNullRunner_Throws() => Assert.Throws<ArgumentNullException>(() => new TaskDelayAction(null!));

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new TaskDelayAction(), Is.Not.Null);
}