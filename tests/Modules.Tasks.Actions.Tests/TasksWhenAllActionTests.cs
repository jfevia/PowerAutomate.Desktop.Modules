// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Tasks.Actions;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions.Tests;

[TestFixture]
public class TasksWhenAllActionTests
{
    [Test]
    public void Execute_WaitsForTasks()
    {
        var runner = new FakeTaskRunner();
        var tasks = new List<TaskObject> { new("One", Task.CompletedTask) };
        var action = new TasksWhenAllAction(runner) { Tasks = tasks };

        action.Execute(new ActionContext());

        Assert.That(runner.LastTasks, Is.EqualTo(tasks));
    }

    [Test]
    public void Execute_WhenRunnerFails_WrapsActionException()
    {
        var action = new TasksWhenAllAction(new FakeTaskRunner { WhenAllException = new InvalidOperationException("Failed") }) { Tasks = new List<TaskObject>() };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));

        Assert.That(exception!.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Constructor_WithNullRunner_Throws() => Assert.Throws<ArgumentNullException>(() => new TasksWhenAllAction(null!));

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new TasksWhenAllAction(), Is.Not.Null);
}