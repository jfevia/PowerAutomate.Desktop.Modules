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
public class GetFolderTasksActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_ReturnsTasksWithoutFilter()
    {
        _test.Service.Root.Tasks.Add(new FakeScheduledTask { Name = "A" });
        _test.Service.Root.Tasks.Add(new FakeScheduledTask { Name = "B" });
        var action = new GetFolderTasksAction(_test.Context) { FolderPath = "\\", Filter = "" };

        action.Execute(ActionHarness.ActionContext());

        Assert.That(action.TaskNames, Is.EqualTo(new[] { "A", "B" }));
        Assert.That(_test.Service.Root.LastFilter, Is.Null);
    }

    [Test]
    public void Execute_ReturnsTasksMatchingFilter()
    {
        _test.Service.Root.Tasks.Add(new FakeScheduledTask { Name = "Alpha" });
        _test.Service.Root.Tasks.Add(new FakeScheduledTask { Name = "Beta" });
        var action = new GetFolderTasksAction(_test.Context) { FolderPath = "\\", Filter = "^A" };

        action.Execute(ActionHarness.ActionContext());

        Assert.That(action.TaskNames, Is.EqualTo(new[] { "Alpha" }));
        Assert.That(_test.Service.Root.LastFilter, Is.Not.Null);
    }

    [Test]
    public void Execute_WhenFolderIsMissing_ThrowsFolderNotFoundError()
    {
        var action = new GetFolderTasksAction(_test.Context) { FolderPath = "Missing" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.FolderNotFound));
    }

    [Test]
    public void Execute_WhenFilterIsInvalid_ThrowsUnknownError()
    {
        var action = new GetFolderTasksAction(_test.Context) { FolderPath = "\\", Filter = "[" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring() => Assert.That(new GetFolderTasksAction().FolderPath, Is.Null);
}
