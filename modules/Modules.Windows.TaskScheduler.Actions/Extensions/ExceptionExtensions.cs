// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Exceptions;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;

public static class ExceptionExtensions
{
    public static ActionException ToActionException(this Exception exception)
    {
        return exception switch
        {
            ActionException actionException => actionException,
            FolderNotFoundException ex => new ActionException(ErrorCodes.FolderNotFound, ex.Message, ex),
            TaskActionNotFoundException ex => new ActionException(ErrorCodes.TaskActionNotFound, ex.Message, ex),
            TaskActionException ex => new ActionException(ErrorCodes.TaskActionUnknown, ex.Message, ex),
            TaskNotFoundException ex => new ActionException(ErrorCodes.TaskNotFound, ex.Message, ex),
            TaskTriggerNotFoundException ex => new ActionException(ErrorCodes.TaskTriggerNotFound, ex.Message, ex),
            TaskTriggerException ex => new ActionException(ErrorCodes.TaskTriggerUnknown, ex.Message, ex),
            _ => new ActionException(ErrorCodes.Unknown, exception.Message, exception)
        };
    }
}
