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
public class TaskTriggerObject
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

    internal TaskTriggerObject(
        string taskName,
        string id,
        TriggerType type,
        bool enabled,
        DateTime startBoundary,
        DateTime endBoundary,
        TimeSpan executionTimeLimit,
        bool repetitionStopAtDurationEnd,
        TimeSpan repetitionInterval,
        TimeSpan repetitionDuration)
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

    public override string ToString()
    {
        return $"{ID} ({Type})";
    }
}