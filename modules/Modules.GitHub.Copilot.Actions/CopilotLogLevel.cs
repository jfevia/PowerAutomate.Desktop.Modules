// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public enum CopilotLogLevel
{
    Unset = 0,  // don't pass --log-level
    Quiet,      // emits --log-level=none
    Errors,     // emits --log-level=error
    Warnings,   // emits --log-level=warning
    Info,
    Debug,
    Verbose     // emits --log-level=all
}
