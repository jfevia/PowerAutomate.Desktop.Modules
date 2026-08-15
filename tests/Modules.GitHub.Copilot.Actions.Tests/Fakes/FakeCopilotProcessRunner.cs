// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions.Tests.Fakes;

internal sealed class FakeCopilotProcessRunner : ICopilotProcessRunner
{
    public Exception? Exception { get; set; }
    public CopilotProcessResult Result { get; set; } = new(7, "out", "err");
    public CopilotProcessStartInfo? StartInfo { get; private set; }

    public CopilotProcessResult Run(CopilotProcessStartInfo startInfo)
    {
        if (Exception != null)
        {
            throw Exception;
        }

        StartInfo = startInfo;
        return Result;
    }
}