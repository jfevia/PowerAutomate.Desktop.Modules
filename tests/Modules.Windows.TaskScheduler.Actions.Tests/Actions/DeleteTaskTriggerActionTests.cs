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
public class DeleteTaskTriggerActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_RemovesTrigger()
    {
        var collection = ((FakeTaskDefinition)_test.Task.Definition).TriggerCollection;
        collection.Triggers.Add(new FakeTaskTrigger { Id = "t" });
        var action = new DeleteTaskTriggerAction(_test.Context) { TaskName = "Task", TriggerId = "t" };

        action.Execute(ActionHarness.ActionContext());

        Assert.That(collection.Triggers, Is.Empty);
    }

    [Test]
    public void Execute_WhenTaskIsMissing_ThrowsTaskNotFoundError()
    {
        var action = new DeleteTaskTriggerAction(_test.Context) { TaskName = "Missing", TriggerId = "t" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskNotFound));
    }

    [Test]
    public void Execute_WhenTriggerIsMissing_ThrowsTaskTriggerNotFoundError()
    {
        var action = new DeleteTaskTriggerAction(_test.Context) { TaskName = "Task", TriggerId = "missing" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskTriggerNotFound));
    }

    [Test]
    public void Execute_WhenRemoveFails_ThrowsTaskTriggerUnknownError()
    {
        var collection = ((FakeTaskDefinition)_test.Task.Definition).TriggerCollection;
        collection.Triggers.Add(new FakeTaskTrigger { Id = "t" });
        collection.RemoveResult = false;
        var action = new DeleteTaskTriggerAction(_test.Context) { TaskName = "Task", TriggerId = "t" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskTriggerUnknown));
    }

    [Test]
    public void Execute_WhenRemoveReportsMissingTrigger_ThrowsTaskTriggerNotFoundError()
    {
        var collection = ((FakeTaskDefinition)_test.Task.Definition).TriggerCollection;
        collection.Triggers.Add(new FakeTaskTrigger { Id = "t" });
        collection.RemoveThrowsFileNotFound = true;
        var action = new DeleteTaskTriggerAction(_test.Context) { TaskName = "Task", TriggerId = "t" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskTriggerNotFound));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring() => Assert.That(new DeleteTaskTriggerAction().TaskName, Is.Null);
}
