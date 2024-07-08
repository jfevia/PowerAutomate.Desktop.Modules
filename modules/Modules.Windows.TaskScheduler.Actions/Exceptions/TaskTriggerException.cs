// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Exceptions;

public class TaskTriggerException : Exception
{
    public string TaskName { get; }
    public string TriggerId { get; }

    public TaskTriggerException(string taskName, string triggerId, string message)
        : base(message)
    {
        TaskName = taskName;
        TriggerId = triggerId;
    }
}