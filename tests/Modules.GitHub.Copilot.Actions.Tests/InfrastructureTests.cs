// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions.Tests;

[TestFixture]
public class InfrastructureTests
{
    [Test]
    public void ProcessStartInfo_StoresValues()
    {
        var startInfo = new CopilotProcessStartInfo("file", "args", "work");

        Assert.That(startInfo.FileName, Is.EqualTo("file"));
        Assert.That(startInfo.Arguments, Is.EqualTo("args"));
        Assert.That(startInfo.WorkingDirectory, Is.EqualTo("work"));
    }

    [Test]
    public void ProcessResult_StoresValues()
    {
        var result = new CopilotProcessResult(2, "out", "err");

        Assert.That(result.ExitCode, Is.EqualTo(2));
        Assert.That(result.StandardOutput, Is.EqualTo("out"));
        Assert.That(result.StandardError, Is.EqualTo("err"));
    }

    [Test]
    public void ActionSelectors_CanBeConstructed()
    {
        Assert.That(new LaunchCopilotCliAutopilotActionSelector(), Is.Not.Null);
        Assert.That(new LaunchCopilotCliInteractiveActionSelector(), Is.Not.Null);
        Assert.That(new LaunchCopilotCliPlanActionSelector(), Is.Not.Null);
    }
}