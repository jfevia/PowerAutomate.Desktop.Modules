// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;
using Newtonsoft.Json;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions;

[JsonObject(MemberSerialization.OptOut)]
[Type(DefaultPropertyVisibility = Visibility.Visible)]
public class TaskObject : IComparable<TaskObject>, IComparable
{
    public string Name { get; private set; } = null!;
    internal Task Task { get; private set; } = null!;

    [JsonConstructor]
    public TaskObject()
    {
    }

    internal TaskObject(string name, Task task)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Task = task ?? throw new ArgumentNullException(nameof(task));
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is TaskObject other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(TaskObject)}");
    }

    public int CompareTo(TaskObject? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
    }

    public override string ToString()
    {
        return Name;
    }
}