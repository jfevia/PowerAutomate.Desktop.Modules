// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;

internal static class SessionStateChangeTypeExtensions
{
    public static TaskSessionStateChangeType ToAbstraction(this SessionStateChangeType value)
    {
        return value switch
        {
            SessionStateChangeType.ConsoleConnect => TaskSessionStateChangeType.ConsoleConnect,
            SessionStateChangeType.ConsoleDisconnect => TaskSessionStateChangeType.ConsoleDisconnect,
            SessionStateChangeType.RemoteConnect => TaskSessionStateChangeType.RemoteConnect,
            SessionStateChangeType.RemoteDisconnect => TaskSessionStateChangeType.RemoteDisconnect,
            SessionStateChangeType.SessionLock => TaskSessionStateChangeType.SessionLock,
            SessionStateChangeType.SessionUnlock => TaskSessionStateChangeType.SessionUnlock,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}