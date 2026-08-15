// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.Win32.TaskScheduler;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Tests.Fakes;

internal sealed class ActionHarness
{
    public ActionHarness()
    {
        Factory = new FakeTaskSchedulerFactory();
        Service = Factory.Service;
        Context = new TaskSchedulerContext(Factory);
        Service.Tasks["Task"] = Task;
        Service.Folders["\\"] = Service.Root;
    }

    public TaskSchedulerContext Context { get; }
    public FakeTaskSchedulerFactory Factory { get; }
    public FakeTaskSchedulerService Service { get; }
    public FakeScheduledTask Task { get; } = new() { Name = "Task", Path = "\\Task", State = TaskState.Ready };

    public static ActionContext ActionContext() => new();

    public static T Throws<T>(System.Action action, string errorCode) where T : Exception
    {
        var exception = Assert.Throws<T>(() => action())!;
        Assert.That(((ActionException)(object)exception).Name, Is.EqualTo(errorCode));
        return exception;
    }
}
