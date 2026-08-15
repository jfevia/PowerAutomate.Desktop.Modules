// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;
using Newtonsoft.Json;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Types;

[JsonObject(MemberSerialization.OptOut)]
[Type(DefaultPropertyVisibility = Visibility.Visible)]
public class TaskTriggerObject : IComparable<TaskTriggerObject>, IComparable
{
    public bool Enabled { get; private set; }
    public DateTime EndBoundary { get; private set; }
    public TimeSpan ExecutionTimeLimit { get; private set; }
    public string ID { get; private set; } = null!;
    public TimeSpan RepetitionDuration { get; private set; }
    public TimeSpan RepetitionInterval { get; private set; }
    public bool RepetitionStopAtDurationEnd { get; private set; }
    public DateTime StartBoundary { get; private set; }
    public string TaskName { get; private set; } = null!;
    public TriggerType Type { get; private set; }

    [JsonConstructor]
    public TaskTriggerObject()
    {
    }

    public TaskTriggerObject(string taskName, string id, TriggerType type, bool enabled, DateTime startBoundary, DateTime endBoundary, TimeSpan executionTimeLimit, bool repetitionStopAtDurationEnd, TimeSpan repetitionInterval, TimeSpan repetitionDuration)
    {
        TaskName = taskName;
        ID = id;
        Type = type;
        Enabled = enabled;
        StartBoundary = startBoundary;
        EndBoundary = endBoundary;
        ExecutionTimeLimit = executionTimeLimit;
        RepetitionStopAtDurationEnd = repetitionStopAtDurationEnd;
        RepetitionInterval = repetitionInterval;
        RepetitionDuration = repetitionDuration;
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is TaskTriggerObject other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TaskTriggerObject)}");
    }

    public int CompareTo(TaskTriggerObject? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var triggerTypeComparison = Type.CompareTo(other.Type);
        if (triggerTypeComparison != 0) return triggerTypeComparison;
        var idComparison = string.Compare(ID, other.ID, StringComparison.InvariantCultureIgnoreCase);
        if (idComparison != 0) return idComparison;
        return string.Compare(TaskName, other.TaskName, StringComparison.InvariantCultureIgnoreCase);
    }

    public override string ToString() => $"{ID} ({Type})";
}
