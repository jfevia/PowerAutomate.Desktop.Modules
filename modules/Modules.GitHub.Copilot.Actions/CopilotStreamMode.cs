// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public enum CopilotStreamMode
{
    Unset = 0,
    Enabled,    // emits --stream=on
    Disabled    // emits --stream=off
}
