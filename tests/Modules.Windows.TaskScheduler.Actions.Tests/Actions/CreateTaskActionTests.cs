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
public class CreateTaskActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_RegistersNewTask()
    {
        var action = new CreateTaskAction(_test.Context) { TaskName = "Created", TargetServer = "s", UserName = "u", AccountDomain = "d", Password = "p" };

        action.Execute(ActionHarness.ActionContext());

        Assert.That(_test.Factory.TargetServer, Is.EqualTo("s"));
        Assert.That(_test.Service.Root.RegisteredTaskName, Is.EqualTo("Created"));
    }

    [Test]
    public void Execute_WhenConnectionFails_ThrowsUnknownError()
    {
        _test.Factory.ConnectException = new InvalidOperationException("boom");
        var action = new CreateTaskAction(_test.Context);

        var exception = Assert.Throws<ActionException>(() => action.Execute(ActionHarness.ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new CreateTaskAction();

        Assert.That(action.TaskName, Is.Null);
    }
}
