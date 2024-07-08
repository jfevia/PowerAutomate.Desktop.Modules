// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Exceptions;

public class TaskActionException : Exception
{
    public string ActionId { get; }
    public string TaskName { get; }

    public TaskActionException(string taskName, string actionId, string message)
        : base(message)
    {
        TaskName = taskName;
        ActionId = actionId;
    }
}