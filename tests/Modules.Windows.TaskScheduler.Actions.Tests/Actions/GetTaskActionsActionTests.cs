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
public class GetTaskActionsActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ReturnsActionIds()
    {
        ((FakeTaskDefinition)_test.Task.Definition).ActionCollection.Actions.Add(new FakeTaskAction { Id = "a" });
        ((FakeTaskDefinition)_test.Task.Definition).ActionCollection.Actions.Add(new FakeTaskAction { Id = "b" });
        var action = new GetTaskActionsAction(_test.Context) { TaskName = "Task" };

        action.Execute(ActionHarness.ActionContext());

        Assert.That(action.Actions, Is.EqualTo(new[] { "a", "b" }));
    }

    [Test]
    public void Execute_WhenTaskIsMissing_ThrowsTaskNotFoundError()
    {
        var action = new GetTaskActionsAction(_test.Context) { TaskName = "Missing" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskNotFound));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring() => Assert.That(new GetTaskActionsAction().TaskName, Is.Null);
}
