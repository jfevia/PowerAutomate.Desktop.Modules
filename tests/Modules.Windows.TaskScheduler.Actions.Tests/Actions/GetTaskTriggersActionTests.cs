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
public class GetTaskTriggersActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ReturnsTriggerIds()
    {
        ((FakeTaskDefinition)_test.Task.Definition).TriggerCollection.Triggers.Add(new FakeTaskTrigger { Id = "a" });
        ((FakeTaskDefinition)_test.Task.Definition).TriggerCollection.Triggers.Add(new FakeTaskTrigger { Id = "b" });
        var action = new GetTaskTriggersAction(_test.Context) { TaskName = "Task" };

        action.Execute(ActionHarness.ActionContext());

        Assert.That(action.Triggers, Is.EqualTo(new[] { "a", "b" }));
    }

    [Test]
    public void Execute_WhenTaskIsMissing_ThrowsTaskNotFoundError()
    {
        var action = new GetTaskTriggersAction(_test.Context) { TaskName = "Missing" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskNotFound));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring() => Assert.That(new GetTaskTriggersAction().TaskName, Is.Null);
}
