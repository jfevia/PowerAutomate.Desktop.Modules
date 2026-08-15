// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public sealed class CopilotProcessResult
{
    public CopilotProcessResult(int exitCode, string standardOutput, string standardError)
    {
        ExitCode = exitCode;
        StandardOutput = standardOutput;
        StandardError = standardError;
    }

    public int ExitCode { get; }
    public string StandardError { get; }
    public string StandardOutput { get; }
}