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
public class DeleteTaskActionActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_RemovesAction()
    {
        var collection = ((FakeTaskDefinition)_test.Task.Definition).ActionCollection;
        collection.Actions.Add(new FakeTaskAction { Id = "a" });
        var action = new DeleteTaskActionAction(_test.Context) { TaskName = "Task", ActionId = "a" };

        action.Execute(ActionHarness.ActionContext());

        Assert.That(collection.Actions, Is.Empty);
    }

    [Test]
    public void Execute_WhenTaskIsMissing_ThrowsTaskNotFoundError()
    {
        var action = new DeleteTaskActionAction(_test.Context) { TaskName = "Missing", ActionId = "a" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskNotFound));
    }

    [Test]
    public void Execute_WhenActionIsMissing_ThrowsTaskActionNotFoundError()
    {
        var action = new DeleteTaskActionAction(_test.Context) { TaskName = "Task", ActionId = "missing" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskActionNotFound));
    }

    [Test]
    public void Execute_WhenRemoveFails_ThrowsTaskActionUnknownError()
    {
        var collection = ((FakeTaskDefinition)_test.Task.Definition).ActionCollection;
        collection.Actions.Add(new FakeTaskAction { Id = "a" });
        collection.RemoveResult = false;
        var action = new DeleteTaskActionAction(_test.Context) { TaskName = "Task", ActionId = "a" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskActionUnknown));
    }

    [Test]
    public void Execute_WhenRemoveReportsMissingAction_ThrowsTaskActionNotFoundError()
    {
        var collection = ((FakeTaskDefinition)_test.Task.Definition).ActionCollection;
        collection.Actions.Add(new FakeTaskAction { Id = "a" });
        collection.RemoveThrowsFileNotFound = true;
        var action = new DeleteTaskActionAction(_test.Context) { TaskName = "Task", ActionId = "a" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskActionNotFound));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring() => Assert.That(new DeleteTaskActionAction().TaskName, Is.Null);
}
