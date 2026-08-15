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
public class GetTaskActionActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ReturnsTaskActionObject()
    {
        ((FakeTaskDefinition)_test.Task.Definition).ActionCollection.Actions.Add(new FakeTaskAction { Id = "a", Type = TaskActionType.Execute });
        var action = new GetTaskActionAction(_test.Context) { TaskName = "Task", ActionId = "a" };

        action.Execute(ActionHarness.ActionContext());

        Assert.That(action.TaskAction.ID, Is.EqualTo("a"));
        Assert.That(action.TaskAction.Type, Is.EqualTo(ActionType.Execute));
    }

    [Test]
    public void Execute_WhenTaskIsMissing_ThrowsTaskNotFoundError()
    {
        var action = new GetTaskActionAction(_test.Context) { TaskName = "Missing", ActionId = "a" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskNotFound));
    }

    [Test]
    public void Execute_WhenActionIsMissing_ThrowsTaskActionNotFoundError()
    {
        var action = new GetTaskActionAction(_test.Context) { TaskName = "Task", ActionId = "missing" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskActionNotFound));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring() => Assert.That(new GetTaskActionAction().TaskName, Is.Null);
}
