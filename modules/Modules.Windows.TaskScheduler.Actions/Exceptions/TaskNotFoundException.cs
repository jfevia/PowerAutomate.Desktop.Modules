// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Exceptions;

public sealed class TaskNotFoundException : Exception
{
    public string TaskName { get; }

    public TaskNotFoundException(string taskName) : base($"Could not find task '{taskName}'")
    {
        TaskName = taskName;
    }
}