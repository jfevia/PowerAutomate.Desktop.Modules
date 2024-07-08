// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;
using Newtonsoft.Json;
using TaskState = PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums.TaskState;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Types;

[JsonObject(MemberSerialization.OptOut)]
[Type(DefaultPropertyVisibility = Visibility.Visible)]
public class TaskObject : IComparable<TaskObject>, IComparable
{
    public bool Enabled { get; private set; }
    public bool IsReadOnly { get; private set; }
    public DateTime LastRunTime { get; private set; }
    public int LastTaskResult { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTime NextRunTime { get; private set; }
    public int NumberOfMissedRuns { get; private set; }
    public string Path { get; private set; } = null!;
    public TaskState State { get; private set; }

    [JsonConstructor]
    public TaskObject()
    {
    }

    internal TaskObject(
        string name,
        string path,
        bool enabled,
        TaskState state,
        bool isReadOnly,
        DateTime lastRunTime,
        int lastTaskResult,
        DateTime nextRunTime,
        int numberOfMissedRuns)
    {
        Name = name;
        Path = path;
        Enabled = enabled;
        State = state;
        IsReadOnly = isReadOnly;
        LastRunTime = lastRunTime;
        LastTaskResult = lastTaskResult;
        NextRunTime = nextRunTime;
        NumberOfMissedRuns = numberOfMissedRuns;
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is TaskObject other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TaskObject)}");
    }

    public int CompareTo(TaskObject? other)
    {
        if (ReferenceEquals(null, other)) return 1;
        if (ReferenceEquals(this, other)) return 0;
        return string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
    }

    public override string ToString()
    {
        return Name;
    }
}