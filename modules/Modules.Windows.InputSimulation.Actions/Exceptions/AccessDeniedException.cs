// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;

/// <summary>
/// Raised when Windows refuses to deliver a message because the target window belongs to a
/// higher integrity level process (User Interface Privilege Isolation).
/// </summary>
public sealed class AccessDeniedException : Exception
{
    public AccessDeniedException()
        : base("Windows blocked the message because the target window belongs to a process running at a higher integrity level. Run this flow elevated to automate an elevated application.")
    {
    }
}
