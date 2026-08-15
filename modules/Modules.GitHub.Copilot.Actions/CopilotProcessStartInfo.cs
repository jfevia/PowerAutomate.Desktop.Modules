// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

public sealed class CopilotProcessStartInfo
{
    public CopilotProcessStartInfo(string fileName, string arguments, string workingDirectory)
    {
        FileName = fileName;
        Arguments = arguments;
        WorkingDirectory = workingDirectory;
    }

    public string Arguments { get; }
    public string FileName { get; }
    public string WorkingDirectory { get; }
}