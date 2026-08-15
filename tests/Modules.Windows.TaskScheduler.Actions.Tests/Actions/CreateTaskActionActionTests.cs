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
public class CreateTaskActionActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_AddsExecAction()
    {
        var action = new CreateTaskActionAction(_test.Context) { TaskName = "Task", Path = "app.exe", Arguments = "-v", WorkingDirectory = "c:\\work" };

        action.Execute(ActionHarness.ActionContext());

        var added = ((FakeTaskDefinition)_test.Task.Definition).ActionCollection.AddedExec;
        Assert.That(added?.Path, Is.EqualTo("app.exe"));
        Assert.That(added?.Arguments, Is.EqualTo("-v"));
        Assert.That(added?.WorkingDirectory, Is.EqualTo("c:\\work"));
    }

    [Test]
    public void Execute_WhenTaskIsMissing_ThrowsTaskNotFoundError()
    {
        var action = new CreateTaskActionAction(_test.Context) { TaskName = "Missing" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.TaskNotFound));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring() => Assert.That(new CreateTaskActionAction().TaskName, Is.Null);
}
