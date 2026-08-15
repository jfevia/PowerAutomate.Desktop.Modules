// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions.Tests;

[TestFixture]
public class LaunchCopilotCliActionTests
{
    [SetUp]
    public void SetUp() => runner = new FakeCopilotProcessRunner();

    private FakeCopilotProcessRunner runner = null!;

    [Test]
    public void Execute_WithDefaults_StartsCopilotAndMapsResult()
    {
        var action = new LaunchCopilotCliAction(runner);

        action.Execute(new ActionContext());

        Assert.That(runner.StartInfo!.FileName, Is.EqualTo("copilot"));
        Assert.That(runner.StartInfo.WorkingDirectory, Is.Empty);
        Assert.That(runner.StartInfo.Arguments, Is.EqualTo("--mode=interactive"));
        Assert.That(action.ExitCode, Is.EqualTo(7));
        Assert.That(action.StandardOutput, Is.EqualTo("out"));
        Assert.That(action.StandardError, Is.EqualTo("err"));
    }

    [Test]
    public void Execute_WithAllEnabledOptions_BuildsArguments()
    {
        var action = CreateFullAction();

        action.Execute(new ActionContext());

        var args = runner.StartInfo!.Arguments;
        Assert.That(runner.StartInfo.FileName, Is.EqualTo("gh-copilot"));
        Assert.That(runner.StartInfo.WorkingDirectory, Is.EqualTo("C:\\repo"));
        Assert.That(args, Does.Contain("--prompt=\"hello world\""));
        Assert.That(args, Does.Contain("--interactive=\"say\\\"hi\""));
        Assert.That(args, Does.Contain("--name=session"));
        Assert.That(args, Does.Contain("--continue"));
        Assert.That(args, Does.Contain("--resume=resume-id"));
        Assert.That(args, Does.Contain("--connect=conn"));
        Assert.That(args, Does.Contain("--mode=autopilot"));
        Assert.That(args, Does.Contain("--autopilot"));
        Assert.That(args, Does.Contain("--plan"));
        Assert.That(args, Does.Contain("--max-autopilot-continues=3"));
        Assert.That(args, Does.Contain("--no-ask-user"));
        Assert.That(args, Does.Contain("--allow-all"));
        Assert.That(args, Does.Contain("--yolo"));
        Assert.That(args, Does.Contain("--allow-all-paths"));
        Assert.That(args, Does.Contain("--allow-all-tools"));
        Assert.That(args, Does.Contain("--allow-all-urls"));
        Assert.That(args, Does.Contain("--allow-tool=tool"));
        Assert.That(args, Does.Contain("--allow-url=https://example.com"));
        Assert.That(args, Does.Contain("--deny-tool=deny"));
        Assert.That(args, Does.Contain("--deny-url=https://deny.example"));
        Assert.That(args, Does.Contain("--available-tools=tools"));
        Assert.That(args, Does.Contain("--excluded-tools=excluded"));
        Assert.That(args, Does.Contain("--model=gpt"));
        Assert.That(args, Does.Contain("--effort=high"));
        Assert.That(args, Does.Contain("--enable-reasoning-summaries"));
        Assert.That(args, Does.Contain("--add-dir=one"));
        Assert.That(args, Does.Contain("--add-dir=two"));
        Assert.That(args, Does.Contain("--disallow-temp-dir"));
        Assert.That(args, Does.Contain("--add-github-mcp-tool=mcp-tool"));
        Assert.That(args, Does.Contain("--add-github-mcp-toolset=mcp-set"));
        Assert.That(args, Does.Contain("--additional-mcp-config=config"));
        Assert.That(args, Does.Contain("--disable-builtin-mcps"));
        Assert.That(args, Does.Contain("--disable-mcp-server=server"));
        Assert.That(args, Does.Contain("--enable-all-github-mcp-tools"));
        Assert.That(args, Does.Contain("--plugin-dir=plugin"));
        Assert.That(args, Does.Contain("--agent=agent"));
        Assert.That(args, Does.Contain("--output-format=json"));
        Assert.That(args, Does.Contain("--silent"));
        Assert.That(args, Does.Contain("--no-color"));
        Assert.That(args, Does.Contain("--banner"));
        Assert.That(args, Does.Contain("--stream=on"));
        Assert.That(args, Does.Contain("--log-dir=logs"));
        Assert.That(args, Does.Contain("--log-level=warning"));
        Assert.That(args, Does.Contain("--share=share"));
        Assert.That(args, Does.Contain("--share-gist"));
        Assert.That(args, Does.Contain("--screen-reader"));
        Assert.That(args, Does.Contain("--experimental"));
        Assert.That(args, Does.Contain("--no-auto-update"));
        Assert.That(args, Does.Contain("--no-custom-instructions"));
        Assert.That(args, Does.Contain("--remote"));
        Assert.That(args, Does.Contain("--bash-env"));
        Assert.That(args, Does.Contain("--mouse"));
        Assert.That(args, Does.Contain("--secret-env-vars=secret"));
        Assert.That(args, Does.Contain("--plain-diff"));
    }

    [Test]
    public void Execute_WithDisabledSwitches_BuildsNegativeArguments()
    {
        var action = new LaunchCopilotCliAction(runner)
        {
            Banner = CopilotSwitch.Disabled,
            BashEnv = CopilotSwitch.Disabled,
            Experimental = CopilotSwitch.Disabled,
            Mouse = CopilotSwitch.Disabled,
            PlainDiff = CopilotSwitch.Disabled,
            Remote = CopilotSwitch.Disabled,
            Stream = CopilotStreamMode.Disabled
        };

        action.Execute(new ActionContext());

        var args = runner.StartInfo!.Arguments;
        Assert.That(args, Does.Contain("--no-banner"));
        Assert.That(args, Does.Contain("--stream=off"));
        Assert.That(args, Does.Contain("--no-experimental"));
        Assert.That(args, Does.Contain("--no-remote"));
        Assert.That(args, Does.Contain("--no-bash-env"));
        Assert.That(args, Does.Contain("--no-mouse"));
        Assert.That(args, Does.Contain("--no-plain-diff"));
    }

    [TestCase(CopilotLogLevel.Quiet, "--log-level=none")]
    [TestCase(CopilotLogLevel.Errors, "--log-level=error")]
    [TestCase(CopilotLogLevel.Verbose, "--log-level=all")]
    [TestCase((CopilotLogLevel)99, "--log-level=99")]
    public void Execute_WithLogLevel_BuildsExpectedArgument(CopilotLogLevel logLevel, string expected)
    {
        var action = new LaunchCopilotCliAction(runner) { LogLevel = logLevel };

        action.Execute(new ActionContext());

        Assert.That(runner.StartInfo!.Arguments, Does.Contain(expected));
    }

    [Test]
    public void Execute_WhenRunnerFails_ThrowsUnknownError()
    {
        runner.Exception = new InvalidOperationException("boom");
        var action = new LaunchCopilotCliAction(runner);

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;
        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void Constructor_WithNullRunner_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LaunchCopilotCliAction(null!));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new LaunchCopilotCliAction();

        Assert.That(action.StandardOutput, Is.Empty);
    }

    private LaunchCopilotCliAction CreateFullAction() => new(runner)
    {
        AddDir = "one, ,two",
        AddGithubMcpTool = "mcp-tool",
        AddGithubMcpToolset = "mcp-set",
        AdditionalMcpConfig = "config",
        Agent = "agent",
        AllowAll = CopilotSwitch.Enabled,
        AllowAllPaths = CopilotSwitch.Enabled,
        AllowAllTools = CopilotSwitch.Enabled,
        AllowAllUrls = CopilotSwitch.Enabled,
        AllowTool = "tool",
        AllowUrl = "https://example.com",
        Autopilot = CopilotSwitch.Enabled,
        AvailableTools = "tools",
        Banner = CopilotSwitch.Enabled,
        BashEnv = CopilotSwitch.Enabled,
        Connect = "conn",
        Continue = CopilotSwitch.Enabled,
        DenyTool = "deny",
        DenyUrl = "https://deny.example",
        DisableBuiltinMcps = CopilotSwitch.Enabled,
        DisableMcpServer = "server",
        DisallowTempDir = CopilotSwitch.Enabled,
        Effort = CopilotEffortLevel.High,
        EnableAllGithubMcpTools = CopilotSwitch.Enabled,
        EnableReasoningSummaries = CopilotSwitch.Enabled,
        ExcludedTools = "excluded",
        ExecutablePath = "gh-copilot",
        Experimental = CopilotSwitch.Enabled,
        InteractivePrompt = "say\"hi",
        LogDir = "logs",
        LogLevel = CopilotLogLevel.Warnings,
        MaxAutopilotContinues = 3,
        Mode = CopilotMode.Autopilot,
        Model = "gpt",
        Mouse = CopilotSwitch.Enabled,
        NoAskUser = CopilotSwitch.Enabled,
        NoAutoUpdate = CopilotSwitch.Enabled,
        NoColor = CopilotSwitch.Enabled,
        NoCustomInstructions = CopilotSwitch.Enabled,
        OutputFormat = CopilotOutputFormat.Json,
        PlainDiff = CopilotSwitch.Enabled,
        Plan = CopilotSwitch.Enabled,
        PluginDir = "plugin",
        Prompt = "hello world",
        Remote = CopilotSwitch.Enabled,
        Resume = "resume-id",
        ScreenReader = CopilotSwitch.Enabled,
        SecretEnvVars = "secret",
        SessionName = "session",
        Share = "share",
        ShareGist = CopilotSwitch.Enabled,
        Silent = CopilotSwitch.Enabled,
        Stream = CopilotStreamMode.Enabled,
        WorkingDirectory = "C:\\repo",
        Yolo = CopilotSwitch.Enabled
    };
}