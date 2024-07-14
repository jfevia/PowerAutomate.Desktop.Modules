// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;

internal static class ActionTypeExtensions
{
    public static ActionType ToAction(this TaskActionType value)
    {
        return value switch
        {
            TaskActionType.Execute => ActionType.Execute,
            TaskActionType.ComHandler => ActionType.ComHandler,
            TaskActionType.SendEmail => ActionType.SendEmail,
            TaskActionType.ShowMessage => ActionType.ShowMessage,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}