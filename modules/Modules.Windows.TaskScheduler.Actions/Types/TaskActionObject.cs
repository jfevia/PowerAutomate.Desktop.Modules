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
public class TaskActionObject : IComparable<TaskActionObject>, IComparable
{
    public string ID { get; private set; } = null!;
    public string TaskName { get; private set; } = null!;
    public ActionType Type { get; private set; }

    [JsonConstructor]
    public TaskActionObject()
    {
    }

    public TaskActionObject(string taskName, string id, ActionType type)
    {
        TaskName = taskName;
        ID = id;
        Type = type;
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is TaskActionObject other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TaskActionObject)}");
    }

    public int CompareTo(TaskActionObject? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var actionTypeComparison = Type.CompareTo(other.Type);
        if (actionTypeComparison != 0) return actionTypeComparison;
        var idComparison = string.Compare(ID, other.ID, StringComparison.InvariantCultureIgnoreCase);
        if (idComparison != 0) return idComparison;
        return string.Compare(TaskName, other.TaskName, StringComparison.InvariantCultureIgnoreCase);
    }

    public override string ToString()
    {
        return $"{ID} ({Type})";
    }
}