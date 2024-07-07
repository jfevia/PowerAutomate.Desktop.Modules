// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

public class TaskActionNotFoundException : TaskActionException
{
    public TaskActionNotFoundException(string taskName, string actionId)
        : base(taskName, actionId, $"Could not find action '{actionId} in task '{taskName}'")
    {
    }
}