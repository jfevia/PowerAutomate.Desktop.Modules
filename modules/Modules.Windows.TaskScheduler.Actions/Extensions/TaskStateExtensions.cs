// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;

internal static class TaskStateExtensions
{
    public static TaskState ToAction(this Microsoft.Win32.TaskScheduler.TaskState value)
    {
        return value switch
        {
            Microsoft.Win32.TaskScheduler.TaskState.Unknown => TaskState.Unknown,
            Microsoft.Win32.TaskScheduler.TaskState.Disabled => TaskState.Disabled,
            Microsoft.Win32.TaskScheduler.TaskState.Queued => TaskState.Queued,
            Microsoft.Win32.TaskScheduler.TaskState.Ready => TaskState.Ready,
            Microsoft.Win32.TaskScheduler.TaskState.Running => TaskState.Running,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}