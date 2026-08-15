// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.Win32.TaskScheduler;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Actions;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Tests.Fakes;
using NativeTaskState = Microsoft.Win32.TaskScheduler.TaskState;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Tests.Actions;

[TestFixture]
public class GetTaskTriggerActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ReturnsTaskTriggerObject()
    {
        ((FakeTaskDefinition)_test.Task.Definition).TriggerCollection.Triggers.Add(new FakeTaskTrigger { Id = "t", Type = TaskTriggerType.Time });
        var action = new GetTaskTriggerAction(_test.Context) { TaskName = "Task", TriggerId = "t" };

        action.Execute(ActionHarness.ActionContext());

        Assert.That(action.TaskTrigger.ID, Is.EqualTo("t"));
        Assert.That(action.TaskTrigger.Type, Is.EqualTo(TriggerType.Time));
    }

    [Test]
    public void Execute_WhenTaskIsMissing_ThrowsTaskNotFoundError()
    {
        var action = new GetTaskTriggerAction(_test.Context) { TaskName = "Missing", TriggerId = "t" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskNotFound));
    }

    [Test]
    public void Execute_WhenTriggerIsMissing_ThrowsTaskTriggerNotFoundError()
    {
        var action = new GetTaskTriggerAction(_test.Context) { TaskName = "Task", TriggerId = "missing" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskTriggerNotFound));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring() => Assert.That(new GetTaskTriggerAction().TaskName, Is.Null);
}
