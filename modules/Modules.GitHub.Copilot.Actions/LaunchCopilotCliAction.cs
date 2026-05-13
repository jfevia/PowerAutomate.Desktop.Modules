// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    // ── General ────────────────────────────────────────────────────────────────
    [InputArgument(Order = 1, Required = false, Group = Groups.General)]
    public string ExecutablePath { get; set; } = string.Empty;

    [InputArgument(Order = 2, Required = false, Group = Groups.General)]
    public string WorkingDirectory { get; set; } = string.Empty;

    // ── Session ────────────────────────────────────────────────────────────────
    [InputArgument(Order = 3, Required = false, Group = Groups.Session)]
    public string Prompt { get; set; } = string.Empty;

    [InputArgument(Order = 4, Required = false, Group = Groups.Session)]
    public string InteractivePrompt { get; set; } = string.Empty;

    [InputArgument(Order = 5, Required = false, Group = Groups.Session)]
    public string SessionName { get; set; } = string.Empty;

    [InputArgument(Order = 6, Required = false, Group = Groups.Session)]
    public bool? Continue { get; set; }

    [InputArgument(Order = 7, Required = false, Group = Groups.Session)]
    public string Resume { get; set; } = string.Empty;

    [InputArgument(Order = 8, Required = false, Group = Groups.Session)]
    public string Connect { get; set; } = string.Empty;

    // ── Mode ───────────────────────────────────────────────────────────────────
    [InputArgument(Order = 9, Group = Groups.Mode)]
    [DefaultValue(CopilotMode.Interactive)]
    public CopilotMode Mode { get; set; } = CopilotMode.Interactive;

    [InputArgument(Order = 10, Required = false, Group = Groups.Mode)]
    public bool? Autopilot { get; set; }

    [InputArgument(Order = 11, Required = false, Group = Groups.Mode)]
    public bool? Plan { get; set; }

    [InputArgument(Order = 12, Required = false, Group = Groups.Mode)]
    public int? MaxAutopilotContinues { get; set; }

    [InputArgument(Order = 13, Required = false, Group = Groups.Mode)]
    public bool? NoAskUser { get; set; }

    // ── Permissions ────────────────────────────────────────────────────────────
    [InputArgument(Order = 14, Required = false, Group = Groups.Permissions)]
    public bool? AllowAll { get; set; }

    [InputArgument(Order = 15, Required = false, Group = Groups.Permissions)]
    public bool? Yolo { get; set; }

    [InputArgument(Order = 16, Required = false, Group = Groups.Permissions)]
    public bool? AllowAllPaths { get; set; }

    [InputArgument(Order = 17, Required = false, Group = Groups.Permissions)]
    public bool? AllowAllTools { get; set; }

    [InputArgument(Order = 18, Required = false, Group = Groups.Permissions)]
    public bool? AllowAllUrls { get; set; }

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

    // ── Model ──────────────────────────────────────────────────────────────────
    [InputArgument(Order = 25, Required = false, Group = Groups.Model)]
    public string Model { get; set; } = string.Empty;

    [InputArgument(Order = 26, Required = false, Group = Groups.Model)]
    public CopilotEffortLevel? Effort { get; set; }

    [InputArgument(Order = 27, Required = false, Group = Groups.Model)]
    public bool? EnableReasoningSummaries { get; set; }

    // ── MCP & Plugins ──────────────────────────────────────────────────────────
    [InputArgument(Order = 28, Required = false, Group = Groups.Mcp)]
    public string AddDir { get; set; } = string.Empty;

    [InputArgument(Order = 29, Required = false, Group = Groups.Mcp)]
    public bool? DisallowTempDir { get; set; }

    [InputArgument(Order = 30, Required = false, Group = Groups.Mcp)]
    public string AddGithubMcpTool { get; set; } = string.Empty;

    [InputArgument(Order = 31, Required = false, Group = Groups.Mcp)]
    public string AddGithubMcpToolset { get; set; } = string.Empty;

    [InputArgument(Order = 32, Required = false, Group = Groups.Mcp)]
    public string AdditionalMcpConfig { get; set; } = string.Empty;

    [InputArgument(Order = 33, Required = false, Group = Groups.Mcp)]
    public bool? DisableBuiltinMcps { get; set; }

    [InputArgument(Order = 34, Required = false, Group = Groups.Mcp)]
    public string DisableMcpServer { get; set; } = string.Empty;

    [InputArgument(Order = 35, Required = false, Group = Groups.Mcp)]
    public bool? EnableAllGithubMcpTools { get; set; }

    [InputArgument(Order = 36, Required = false, Group = Groups.Mcp)]
    public string PluginDir { get; set; } = string.Empty;

    [InputArgument(Order = 37, Required = false, Group = Groups.Mcp)]
    public string Agent { get; set; } = string.Empty;

    // ── Display & Output ───────────────────────────────────────────────────────
    [InputArgument(Order = 38, Required = false, Group = Groups.Display)]
    public CopilotOutputFormat? OutputFormat { get; set; }

    [InputArgument(Order = 39, Required = false, Group = Groups.Display)]
    public bool? Silent { get; set; }

    [InputArgument(Order = 40, Required = false, Group = Groups.Display)]
    public bool? NoColor { get; set; }

    [InputArgument(Order = 41, Required = false, Group = Groups.Display)]
    public bool? Banner { get; set; }

    [InputArgument(Order = 42, Required = false, Group = Groups.Display)]
    public bool? NoBanner { get; set; }

    [InputArgument(Order = 43, Required = false, Group = Groups.Display)]
    public CopilotStreamMode? Stream { get; set; }

    [InputArgument(Order = 44, Required = false, Group = Groups.Display)]
    public string LogDir { get; set; } = string.Empty;

    [InputArgument(Order = 45, Required = false, Group = Groups.Display)]
    public CopilotLogLevel? LogLevel { get; set; }

    [InputArgument(Order = 46, Required = false, Group = Groups.Display)]
    public string Share { get; set; } = string.Empty;

    [InputArgument(Order = 47, Required = false, Group = Groups.Display)]
    public bool? ShareGist { get; set; }

    [InputArgument(Order = 48, Required = false, Group = Groups.Display)]
    public bool? ScreenReader { get; set; }

    // ── Advanced ───────────────────────────────────────────────────────────────
    [InputArgument(Order = 49, Required = false, Group = Groups.Advanced)]
    public bool? Experimental { get; set; }

    [InputArgument(Order = 50, Required = false, Group = Groups.Advanced)]
    public bool? NoExperimental { get; set; }

    [InputArgument(Order = 51, Required = false, Group = Groups.Advanced)]
    public bool? NoAutoUpdate { get; set; }

    [InputArgument(Order = 52, Required = false, Group = Groups.Advanced)]
    public bool? NoCustomInstructions { get; set; }

    [InputArgument(Order = 53, Required = false, Group = Groups.Advanced)]
    public bool? Remote { get; set; }

    [InputArgument(Order = 54, Required = false, Group = Groups.Advanced)]
    public bool? NoRemote { get; set; }

    [InputArgument(Order = 55, Required = false, Group = Groups.Advanced)]
    public bool? BashEnv { get; set; }

    [InputArgument(Order = 56, Required = false, Group = Groups.Advanced)]
    public bool? NoBashEnv { get; set; }

    [InputArgument(Order = 57, Required = false, Group = Groups.Advanced)]
    public bool? Mouse { get; set; }

    [InputArgument(Order = 58, Required = false, Group = Groups.Advanced)]
    public bool? NoMouse { get; set; }

    [InputArgument(Order = 59, Required = false, Group = Groups.Advanced)]
    public string SecretEnvVars { get; set; } = string.Empty;

    [InputArgument(Order = 60, Required = false, Group = Groups.Advanced)]
    public bool? PlainDiff { get; set; }

    [InputArgument(Order = 61, Required = false, Group = Groups.Advanced)]
    public bool? NoPlainDiff { get; set; }

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
            var workDir = string.IsNullOrEmpty(WorkingDirectory) ? null : WorkingDirectory;

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = string.Join(" ", args),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            if (workDir != null)
                startInfo.WorkingDirectory = workDir;

            using var process = Process.Start(startInfo)!;
            var stdOut = process.StandardOutput.ReadToEnd();
            var stdErr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            ExitCode = process.ExitCode;
            StandardOutput = stdOut;
            StandardError = stdErr;
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
        if (Continue == true)                          args.Add("--continue");
        if (!string.IsNullOrEmpty(Resume))            args.Add($"--resume={Quote(Resume)}");
        if (!string.IsNullOrEmpty(Connect))           args.Add($"--connect={Quote(Connect)}");

        // Mode
        args.Add($"--mode={Mode.ToString().ToLowerInvariant()}");
        if (Autopilot == true)                        args.Add("--autopilot");
        if (Plan == true)                             args.Add("--plan");
        if (MaxAutopilotContinues.HasValue)           args.Add($"--max-autopilot-continues={MaxAutopilotContinues.Value}");
        if (NoAskUser == true)                        args.Add("--no-ask-user");

        // Permissions
        if (AllowAll == true)                         args.Add("--allow-all");
        if (Yolo == true)                             args.Add("--yolo");
        if (AllowAllPaths == true)                    args.Add("--allow-all-paths");
        if (AllowAllTools == true)                    args.Add("--allow-all-tools");
        if (AllowAllUrls == true)                     args.Add("--allow-all-urls");
        if (!string.IsNullOrEmpty(AllowTool))         args.Add($"--allow-tool={Quote(AllowTool)}");
        if (!string.IsNullOrEmpty(AllowUrl))          args.Add($"--allow-url={Quote(AllowUrl)}");
        if (!string.IsNullOrEmpty(DenyTool))          args.Add($"--deny-tool={Quote(DenyTool)}");
        if (!string.IsNullOrEmpty(DenyUrl))           args.Add($"--deny-url={Quote(DenyUrl)}");
        if (!string.IsNullOrEmpty(AvailableTools))    args.Add($"--available-tools={Quote(AvailableTools)}");
        if (!string.IsNullOrEmpty(ExcludedTools))     args.Add($"--excluded-tools={Quote(ExcludedTools)}");

        // Model/AI
        if (!string.IsNullOrEmpty(Model))             args.Add($"--model={Quote(Model)}");
        if (Effort.HasValue)                          args.Add($"--effort={Effort.Value.ToString().ToLowerInvariant()}");
        if (EnableReasoningSummaries == true)         args.Add("--enable-reasoning-summaries");

        // Paths/Dirs
        AddMultiValueArgs(args, AddDir, "--add-dir");
        if (DisallowTempDir == true)                  args.Add("--disallow-temp-dir");

        // MCP/Tools
        AddMultiValueArgs(args, AddGithubMcpTool, "--add-github-mcp-tool");
        AddMultiValueArgs(args, AddGithubMcpToolset, "--add-github-mcp-toolset");
        if (!string.IsNullOrEmpty(AdditionalMcpConfig)) args.Add($"--additional-mcp-config={Quote(AdditionalMcpConfig)}");
        if (DisableBuiltinMcps == true)               args.Add("--disable-builtin-mcps");
        AddMultiValueArgs(args, DisableMcpServer, "--disable-mcp-server");
        if (EnableAllGithubMcpTools == true)          args.Add("--enable-all-github-mcp-tools");
        AddMultiValueArgs(args, PluginDir, "--plugin-dir");
        if (!string.IsNullOrEmpty(Agent))             args.Add($"--agent={Quote(Agent)}");

        // Output/Display
        if (OutputFormat.HasValue)                    args.Add($"--output-format={OutputFormat.Value.ToString().ToLowerInvariant()}");
        if (Silent == true)                           args.Add("--silent");
        if (NoColor == true)                          args.Add("--no-color");
        if (Banner == true)                           args.Add("--banner");
        if (NoBanner == true)                         args.Add("--no-banner");
        if (Stream.HasValue)                          args.Add($"--stream={Stream.Value.ToString().ToLowerInvariant()}");
        if (!string.IsNullOrEmpty(LogDir))            args.Add($"--log-dir={Quote(LogDir)}");
        if (LogLevel.HasValue)                        args.Add($"--log-level={LogLevel.Value.ToString().ToLowerInvariant()}");
        if (!string.IsNullOrEmpty(Share))             args.Add($"--share={Quote(Share)}");
        if (ShareGist == true)                        args.Add("--share-gist");
        if (ScreenReader == true)                     args.Add("--screen-reader");

        // Advanced
        if (Experimental == true)       args.Add("--experimental");
        if (NoExperimental == true)     args.Add("--no-experimental");
        if (NoAutoUpdate == true)       args.Add("--no-auto-update");
        if (NoCustomInstructions == true) args.Add("--no-custom-instructions");
        if (Remote == true)             args.Add("--remote");
        if (NoRemote == true)           args.Add("--no-remote");
        if (BashEnv == true)            args.Add("--bash-env");
        if (NoBashEnv == true)          args.Add("--no-bash-env");
        if (Mouse == true)              args.Add("--mouse");
        if (NoMouse == true)            args.Add("--no-mouse");
        AddMultiValueArgs(args, SecretEnvVars, "--secret-env-vars");
        if (PlainDiff == true)          args.Add("--plain-diff");
        if (NoPlainDiff == true)        args.Add("--no-plain-diff");

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
