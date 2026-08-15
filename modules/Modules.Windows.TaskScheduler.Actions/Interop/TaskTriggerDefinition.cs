// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums;
using DayOfWeek = PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums.DayOfWeek;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

public sealed class TaskTriggerDefinition
{
    public short DaysInterval { get; set; }
    public IReadOnlyList<int> DaysOfMonth { get; set; } = Array.Empty<int>();
    public IReadOnlyList<DayOfWeek> DaysOfWeek { get; set; } = Array.Empty<DayOfWeek>();
    public TimeSpan Delay { get; set; }
    public bool Enabled { get; set; }
    public DateTime EndBoundary { get; set; }
    public string Id { get; set; } = string.Empty;
    public IReadOnlyList<MonthOfYear> MonthsOfYear { get; set; } = Array.Empty<MonthOfYear>();
    public TimeSpan RandomDelay { get; set; }
    public TimeSpan RepetitionDuration { get; set; }
    public TimeSpan RepetitionInterval { get; set; }
    public bool RepetitionStopAtDurationEnd { get; set; }
    public bool RunOnLastDayOfMonth { get; set; }
    public DateTime StartBoundary { get; set; }
    public SessionStateChangeType State { get; set; }
    public string Subscription { get; set; } = string.Empty;
    public TimeSpan Timeout { get; set; }
    public TriggerType Type { get; set; }
    public string UserId { get; set; } = string.Empty;
    public IReadOnlyDictionary<string, string> ValueQueries { get; set; } = new Dictionary<string, string>();
    public short WeeksInterval { get; set; }
    public IReadOnlyList<WeekOfMonth> WeeksOfMonth { get; set; } = Array.Empty<WeekOfMonth>();
}
