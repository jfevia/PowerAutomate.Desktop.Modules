// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Exceptions;

public class TaskTriggerNotFoundException : TaskTriggerException
{
    public TaskTriggerNotFoundException(string taskName, string triggerId)
        : base(taskName, triggerId, $"Could not find trigger '{triggerId} in task '{taskName}'")
    {
    }
}