// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;

internal static class TriggerTypeExtensions
{
    public static TriggerType ToAction(this TaskTriggerType value)
    {
        return value switch
        {
            TaskTriggerType.Event => TriggerType.Event,
            TaskTriggerType.Time => TriggerType.Time,
            TaskTriggerType.Daily => TriggerType.Daily,
            TaskTriggerType.Weekly => TriggerType.Weekly,
            TaskTriggerType.Monthly => TriggerType.Monthly,
            TaskTriggerType.MonthlyDOW => TriggerType.MonthlyDayOfWeek,
            TaskTriggerType.Idle => TriggerType.Idle,
            TaskTriggerType.Registration => TriggerType.Registration,
            TaskTriggerType.Boot => TriggerType.Boot,
            TaskTriggerType.Logon => TriggerType.Logon,
            TaskTriggerType.SessionStateChange => TriggerType.SessionStateChange,
            TaskTriggerType.Custom => throw new NotSupportedException($"Task trigger type '{value}' is not supported"),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}