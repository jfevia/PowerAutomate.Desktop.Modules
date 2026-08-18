// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.GitHub.Copilot.Actions;

[Action(Id = "LaunchCopilotCli")]
[Throws(ErrorCodes.Unknown)]
[Group(Name = Groups.General,     Order = 1, IsDefault = true)]
[Group(Name = Groups.Session,     Order = 2)]
[Group(Name = Groups.Mode,        Order = 3)]
[Group(Name = Groups.Permissions, Order = 4)]
[Group(Name = Groups.Model,       Order = 5)]
[Group(Name = Groups.Mcp,         Order = 6)]
[Group(Name = Groups.Display,     Order = 7)]
[Group(Name = Groups.Advanced,    Order = 8)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class LaunchCopilotCliAction : ActionBase
{
    private readonly ICopilotProcessRunner processRunner;

    public LaunchCopilotCliAction() : this(new CopilotProcessRunner())
    {
    }

    internal LaunchCopilotCliAction(ICopilotProcessRunner processRunner)
    {
        this.processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    }

    // -- General ----------------------------------------------------------------
    [InputArgument(Order = 1, Required = false, Group = Groups.General)]
    public string ExecutablePath { get; set; } = string.Empty;

    [InputArgument(Order = 2, Required = false, Group = Groups.General)]
    public string WorkingDirectory { get; set; } = string.Empty;

    // -- Session ----------------------------------------------------------------
    [InputArgument(Order = 3, Required = false, Group = Groups.Session)]
    public string Prompt { get; set; } = string.Empty;

    [InputArgument(Order = 4, Required = false, Group = Groups.Session)]
    public string InteractivePrompt { get; set; } = string.Empty;

    [InputArgument(Order = 5, Required = false, Group = Groups.Session)]
    public string SessionName { get; set; } = string.Empty;

    [InputArgument(Order = 6, Group = Groups.Session)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch Continue { get; set; }

    [InputArgument(Order = 7, Required = false, Group = Groups.Session)]
    public string Resume { get; set; } = string.Empty;

    [InputArgument(Order = 8, Required = false, Group = Groups.Session)]
    public string Connect { get; set; } = string.Empty;

    // -- Mode -------------------------------------------------------------------
    [InputArgument(Order = 9, Group = Groups.Mode)]
    [DefaultValue(CopilotMode.Interactive)]
    public CopilotMode Mode { get; set; } = CopilotMode.Interactive;

    [InputArgument(Order = 10, Group = Groups.Mode)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch Autopilot { get; set; }

    [InputArgument(Order = 11, Group = Groups.Mode)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch Plan { get; set; }

    [InputArgument(Order = 12, Required = false, Group = Groups.Mode)]
    public int? MaxAutopilotContinues { get; set; }

    [InputArgument(Order = 13, Group = Groups.Mode)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch NoAskUser { get; set; }

    // -- Permissions ------------------------------------------------------------
    [InputArgument(Order = 14, Group = Groups.Permissions)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch AllowAll { get; set; }

    [InputArgument(Order = 15, Group = Groups.Permissions)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch Yolo { get; set; }

    [InputArgument(Order = 16, Group = Groups.Permissions)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch AllowAllPaths { get; set; }

    [InputArgument(Order = 17, Group = Groups.Permissions)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch AllowAllTools { get; set; }

    [InputArgument(Order = 18, Group = Groups.Permissions)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch AllowAllUrls { get; set; }

    [InputArgument(Order = 19, Required = false, Group = Groups.Permissions)]
    public string AllowTool { get; set; } = string.Empty;

    [InputArgument(Order = 20, Required = false, Group = Groups.Permissions)]
    public string AllowUrl { get; set; } = string.Empty;

    [InputArgument(Order = 21, Required = false, Group = Groups.Permissions)]
    public string DenyTool { get; set; } = string.Empty;

    [InputArgument(Order = 22, Required = false, Group = Groups.Permissions)]
    public string DenyUrl { get; set; } = string.Empty;

    [InputArgument(Order = 23, Required = false, Group = Groups.Permissions)]
    public string AvailableTools { get; set; } = string.Empty;

    [InputArgument(Order = 24, Required = false, Group = Groups.Permissions)]
    public string ExcludedTools { get; set; } = string.Empty;

    // -- Model ------------------------------------------------------------------
    [InputArgument(Order = 25, Required = false, Group = Groups.Model)]
    public string Model { get; set; } = string.Empty;

    [InputArgument(Order = 26, Group = Groups.Model)]
    [DefaultValue(CopilotEffortLevel.Unset)]
    public CopilotEffortLevel Effort { get; set; }

    [InputArgument(Order = 27, Group = Groups.Model)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch EnableReasoningSummaries { get; set; }

    // -- MCP & Plugins ----------------------------------------------------------
    [InputArgument(Order = 28, Required = false, Group = Groups.Mcp)]
    public string AddDir { get; set; } = string.Empty;

    [InputArgument(Order = 29, Group = Groups.Mcp)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch DisallowTempDir { get; set; }

    [InputArgument(Order = 30, Required = false, Group = Groups.Mcp)]
    public string AddGithubMcpTool { get; set; } = string.Empty;

    [InputArgument(Order = 31, Required = false, Group = Groups.Mcp)]
    public string AddGithubMcpToolset { get; set; } = string.Empty;

    [InputArgument(Order = 32, Required = false, Group = Groups.Mcp)]
    public string AdditionalMcpConfig { get; set; } = string.Empty;

    [InputArgument(Order = 33, Group = Groups.Mcp)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch DisableBuiltinMcps { get; set; }

    [InputArgument(Order = 34, Required = false, Group = Groups.Mcp)]
    public string DisableMcpServer { get; set; } = string.Empty;

    [InputArgument(Order = 35, Group = Groups.Mcp)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch EnableAllGithubMcpTools { get; set; }

    [InputArgument(Order = 36, Required = false, Group = Groups.Mcp)]
    public string PluginDir { get; set; } = string.Empty;

    [InputArgument(Order = 37, Required = false, Group = Groups.Mcp)]
    public string Agent { get; set; } = string.Empty;

    // -- Display & Output -------------------------------------------------------
    [InputArgument(Order = 38, Group = Groups.Display)]
    [DefaultValue(CopilotOutputFormat.Unset)]
    public CopilotOutputFormat OutputFormat { get; set; }

    [InputArgument(Order = 39, Group = Groups.Display)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch Silent { get; set; }

    [InputArgument(Order = 40, Group = Groups.Display)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch NoColor { get; set; }

    [InputArgument(Order = 41, Group = Groups.Display)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch Banner { get; set; }

    [InputArgument(Order = 43, Group = Groups.Display)]
    [DefaultValue(CopilotStreamMode.Unset)]
    public CopilotStreamMode Stream { get; set; }

    [InputArgument(Order = 44, Required = false, Group = Groups.Display)]
    public string LogDir { get; set; } = string.Empty;

    [InputArgument(Order = 45, Group = Groups.Display)]
    [DefaultValue(CopilotLogLevel.Unset)]
    public CopilotLogLevel LogLevel { get; set; }

    [InputArgument(Order = 46, Required = false, Group = Groups.Display)]
    public string Share { get; set; } = string.Empty;

    [InputArgument(Order = 47, Group = Groups.Display)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch ShareGist { get; set; }

    [InputArgument(Order = 48, Group = Groups.Display)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch ScreenReader { get; set; }

    // -- Advanced ---------------------------------------------------------------
    [InputArgument(Order = 49, Group = Groups.Advanced)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch Experimental { get; set; }

    [InputArgument(Order = 51, Group = Groups.Advanced)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch NoAutoUpdate { get; set; }

    [InputArgument(Order = 52, Group = Groups.Advanced)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch NoCustomInstructions { get; set; }

    [InputArgument(Order = 53, Group = Groups.Advanced)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch Remote { get; set; }

    [InputArgument(Order = 54, Group = Groups.Advanced)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch BashEnv { get; set; }

    [InputArgument(Order = 55, Group = Groups.Advanced)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch Mouse { get; set; }

    [InputArgument(Order = 56, Required = false, Group = Groups.Advanced)]
    public string SecretEnvVars { get; set; } = string.Empty;

    [InputArgument(Order = 57, Group = Groups.Advanced)]
    [DefaultValue(CopilotSwitch.Unset)]
    public CopilotSwitch PlainDiff { get; set; }

    // Output arguments
    [OutputArgument(Order = 1)]
    public int ExitCode { get; set; }

    [OutputArgument(Order = 2)]
    public string StandardOutput { get; set; } = string.Empty;

    [OutputArgument(Order = 3)]
    public string StandardError { get; set; } = string.Empty;

    public override void Execute(ActionContext context)
    {
        try
        {
            var args = BuildArguments();
            var exePath = string.IsNullOrEmpty(ExecutablePath) ? "copilot" : ExecutablePath;
            var workDir = string.IsNullOrEmpty(WorkingDirectory) ? string.Empty : WorkingDirectory;
            var result = processRunner.Run(new CopilotProcessStartInfo(exePath, string.Join(" ", args), workDir));

            ExitCode = result.ExitCode;
            StandardOutput = result.StandardOutput;
            StandardError = result.StandardError;
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }

    private List<string> BuildArguments()
    {
        var args = new List<string>();

        // Prompt/Session
        if (!string.IsNullOrEmpty(Prompt))            args.Add($"--prompt={Quote(Prompt)}");
        if (!string.IsNullOrEmpty(InteractivePrompt)) args.Add($"--interactive={Quote(InteractivePrompt)}");
        if (!string.IsNullOrEmpty(SessionName))       args.Add($"--name={Quote(SessionName)}");
        if (Continue == CopilotSwitch.Enabled)                  args.Add("--continue");
        if (!string.IsNullOrEmpty(Resume))            args.Add($"--resume={Quote(Resume)}");
        if (!string.IsNullOrEmpty(Connect))           args.Add($"--connect={Quote(Connect)}");

        // Mode
        args.Add($"--mode={Mode.ToString().ToLowerInvariant()}");
        if (Autopilot == CopilotSwitch.Enabled)                 args.Add("--autopilot");
        if (Plan == CopilotSwitch.Enabled)                       args.Add("--plan");
        if (MaxAutopilotContinues.HasValue)           args.Add($"--max-autopilot-continues={MaxAutopilotContinues.Value}");
        if (NoAskUser == CopilotSwitch.Enabled)                  args.Add("--no-ask-user");

        // Permissions
        if (AllowAll == CopilotSwitch.Enabled)                   args.Add("--allow-all");
        if (Yolo == CopilotSwitch.Enabled)                       args.Add("--yolo");
        if (AllowAllPaths == CopilotSwitch.Enabled)              args.Add("--allow-all-paths");
        if (AllowAllTools == CopilotSwitch.Enabled)              args.Add("--allow-all-tools");
        if (AllowAllUrls == CopilotSwitch.Enabled)               args.Add("--allow-all-urls");
        if (!string.IsNullOrEmpty(AllowTool))         args.Add($"--allow-tool={Quote(AllowTool)}");
        if (!string.IsNullOrEmpty(AllowUrl))          args.Add($"--allow-url={Quote(AllowUrl)}");
        if (!string.IsNullOrEmpty(DenyTool))          args.Add($"--deny-tool={Quote(DenyTool)}");
        if (!string.IsNullOrEmpty(DenyUrl))           args.Add($"--deny-url={Quote(DenyUrl)}");
        if (!string.IsNullOrEmpty(AvailableTools))    args.Add($"--available-tools={Quote(AvailableTools)}");
        if (!string.IsNullOrEmpty(ExcludedTools))     args.Add($"--excluded-tools={Quote(ExcludedTools)}");

        // Model/AI
        if (!string.IsNullOrEmpty(Model))             args.Add($"--model={Quote(Model)}");
        if (Effort != CopilotEffortLevel.Unset)       args.Add($"--effort={Effort.ToString().ToLowerInvariant()}");
        if (EnableReasoningSummaries == CopilotSwitch.Enabled)   args.Add("--enable-reasoning-summaries");

        // Paths/Dirs
        AddMultiValueArgs(args, AddDir, "--add-dir");
        if (DisallowTempDir == CopilotSwitch.Enabled)            args.Add("--disallow-temp-dir");

        // MCP/Tools
        AddMultiValueArgs(args, AddGithubMcpTool, "--add-github-mcp-tool");
        AddMultiValueArgs(args, AddGithubMcpToolset, "--add-github-mcp-toolset");
        if (!string.IsNullOrEmpty(AdditionalMcpConfig)) args.Add($"--additional-mcp-config={Quote(AdditionalMcpConfig)}");
        if (DisableBuiltinMcps == CopilotSwitch.Enabled)         args.Add("--disable-builtin-mcps");
        AddMultiValueArgs(args, DisableMcpServer, "--disable-mcp-server");
        if (EnableAllGithubMcpTools == CopilotSwitch.Enabled)    args.Add("--enable-all-github-mcp-tools");
        AddMultiValueArgs(args, PluginDir, "--plugin-dir");
        if (!string.IsNullOrEmpty(Agent))             args.Add($"--agent={Quote(Agent)}");

        // Output/Display
        if (OutputFormat != CopilotOutputFormat.Unset) args.Add($"--output-format={OutputFormat.ToString().ToLowerInvariant()}");
        if (Silent == CopilotSwitch.Enabled)                     args.Add("--silent");
        if (NoColor == CopilotSwitch.Enabled)                    args.Add("--no-color");
        if (Banner == CopilotSwitch.Enabled)         args.Add("--banner");
        else if (Banner == CopilotSwitch.Disabled)   args.Add("--no-banner");
        if (Stream == CopilotStreamMode.Enabled)  args.Add("--stream=on");
        else if (Stream == CopilotStreamMode.Disabled) args.Add("--stream=off");
        if (!string.IsNullOrEmpty(LogDir))            args.Add($"--log-dir={Quote(LogDir)}");
        if (LogLevel != CopilotLogLevel.Unset)
        {
            var levelStr = LogLevel switch
            {
                CopilotLogLevel.Quiet    => "none",
                CopilotLogLevel.Errors   => "error",
                CopilotLogLevel.Warnings => "warning",
                CopilotLogLevel.Verbose  => "all",
                _                       => LogLevel.ToString().ToLowerInvariant()
            };
            args.Add($"--log-level={levelStr}");
        }
        if (!string.IsNullOrEmpty(Share))             args.Add($"--share={Quote(Share)}");
        if (ShareGist == CopilotSwitch.Enabled)                  args.Add("--share-gist");
        if (ScreenReader == CopilotSwitch.Enabled)               args.Add("--screen-reader");

        // Advanced
        if (Experimental == CopilotSwitch.Enabled)       args.Add("--experimental");
        else if (Experimental == CopilotSwitch.Disabled) args.Add("--no-experimental");
        if (NoAutoUpdate == CopilotSwitch.Enabled)       args.Add("--no-auto-update");
        if (NoCustomInstructions == CopilotSwitch.Enabled) args.Add("--no-custom-instructions");
        if (Remote == CopilotSwitch.Enabled)             args.Add("--remote");
        else if (Remote == CopilotSwitch.Disabled)       args.Add("--no-remote");
        if (BashEnv == CopilotSwitch.Enabled)            args.Add("--bash-env");
        else if (BashEnv == CopilotSwitch.Disabled)      args.Add("--no-bash-env");
        if (Mouse == CopilotSwitch.Enabled)              args.Add("--mouse");
        else if (Mouse == CopilotSwitch.Disabled)        args.Add("--no-mouse");
        AddMultiValueArgs(args, SecretEnvVars, "--secret-env-vars");
        if (PlainDiff == CopilotSwitch.Enabled)          args.Add("--plain-diff");
        else if (PlainDiff == CopilotSwitch.Disabled)    args.Add("--no-plain-diff");

        return args;
    }

    private static void AddMultiValueArgs(List<string> args, string value, string flag)
    {
        if (string.IsNullOrEmpty(value)) return;
        foreach (var item in value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = item.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                args.Add($"{flag}={Quote(trimmed)}");
        }
    }

    private static string Quote(string value)
    {
        if (value.Contains(" ") || value.Contains("\""))
            return $"\"{value.Replace("\"", "\\\"")}\"";
        return value;
    }
}
