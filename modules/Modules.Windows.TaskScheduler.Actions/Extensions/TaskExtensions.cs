// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;

public static class TaskExtensions
{
    public static TaskActionObject ToAction(this ITaskAction value, string taskName)
    {
        return new TaskActionObject(taskName, value.Id, value.Type.ToAction());
    }

    public static TaskObject ToAction(this IScheduledTask value)
    {
        return new TaskObject(
            value.Name,
            value.Path,
            value.Enabled,
            value.State.ToAction(),
            value.IsReadOnly,
            value.LastRunTime,
            value.LastTaskResult,
            value.NextRunTime,
            value.NumberOfMissedRuns);
    }

    public static TaskTriggerObject ToAction(this ITaskTrigger value, string taskName)
    {
        return new TaskTriggerObject(
            taskName,
            value.Id,
            value.Type.ToAction(),
            value.Enabled,
            value.StartBoundary,
            value.EndBoundary,
            value.ExecutionTimeLimit,
            value.Repetition.StopAtDurationEnd,
            value.Repetition.Interval,
            value.Repetition.Duration);
    }
}
