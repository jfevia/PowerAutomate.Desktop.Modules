// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

// Thin process adapter excluded from unit coverage.
[ExcludeFromCodeCoverage]
internal sealed class CopilotProcessRunner : ICopilotProcessRunner
{
    public CopilotProcessResult Run(CopilotProcessStartInfo startInfo)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = startInfo.FileName,
            Arguments = startInfo.Arguments,
            WorkingDirectory = startInfo.WorkingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = Process.Start(processStartInfo)!;
        var standardOutput = process.StandardOutput.ReadToEnd();
        var standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return new CopilotProcessResult(process.ExitCode, standardOutput, standardError);
    }
}