// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

public sealed class TaskSchedulerContext
{
    public TaskSchedulerContext(ITaskSchedulerFactory schedulerFactory)
    {
        SchedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
    }

    public ITaskSchedulerFactory SchedulerFactory { get; }

    public static TaskSchedulerContext CreateDefault() => new(new NativeTaskSchedulerFactory());
}
