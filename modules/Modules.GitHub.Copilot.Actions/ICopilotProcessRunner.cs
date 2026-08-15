// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public interface ICopilotProcessRunner
{
    CopilotProcessResult Run(CopilotProcessStartInfo startInfo);
}