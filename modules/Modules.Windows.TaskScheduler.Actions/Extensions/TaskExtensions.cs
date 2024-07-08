// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;

internal static class TaskExtensions
{
    public static TaskActionObject ToAction(this Action value, string taskName)
    {
        return new TaskActionObject(taskName, value.Id, value.ActionType.ToAction());
    }

    public static TaskObject ToAction(this Task value)
    {
        return new TaskObject(
            value.Name,
            value.Path,
            value.Enabled,
            value.State.ToAction(),
            value.ReadOnly,
            value.LastRunTime,
            value.LastTaskResult,
            value.NextRunTime,
            value.NumberOfMissedRuns);
    }

    public static TaskTriggerObject ToAction(this Trigger value, string taskName)
    {
        return new TaskTriggerObject(
            taskName,
            value.Id,
            value.TriggerType.ToAction(),
            value.Enabled,
            value.StartBoundary,
            value.EndBoundary,
            value.ExecutionTimeLimit,
            value.Repetition.StopAtDurationEnd,
            value.Repetition.Interval,
            value.Repetition.Duration);
    }
}