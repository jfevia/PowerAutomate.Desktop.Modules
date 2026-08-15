// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Net.Http;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions;

public interface IGitHubHttpClientFactory
{
    HttpClient CreateClient();
}

public interface IGitHubProcessRunner
{
    GitHubProcessResult Run(GitHubProcessStartInfo startInfo);
}

public interface IGitHubClock
{
    DateTime UtcNow { get; }
    void Sleep(TimeSpan delay);
}

public interface IGitHubClipboard
{
    void SetText(string text);
}

public interface IGitHubBrowserLauncher
{
    void Open(string url);
}

public sealed class GitHubProcessStartInfo
{
    public GitHubProcessStartInfo(string fileName, string arguments)
    {
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
    }

    public string FileName { get; }
    public string Arguments { get; }
}

public sealed class GitHubProcessResult
{
    public GitHubProcessResult(int exitCode, string standardOutput, string standardError)
    {
        ExitCode = exitCode;
        StandardOutput = standardOutput ?? string.Empty;
        StandardError = standardError ?? string.Empty;
    }

    public int ExitCode { get; }
    public string StandardOutput { get; }
    public string StandardError { get; }
}

// Default adapter forwards to HttpClient.
[ExcludeFromCodeCoverage]
internal sealed class GitHubHttpClientFactory : IGitHubHttpClientFactory
{
    public HttpClient CreateClient() => new();
}

// Default adapter forwards to Process.Start.
[ExcludeFromCodeCoverage]
internal sealed class GitHubProcessRunner : IGitHubProcessRunner
{
    public GitHubProcessResult Run(GitHubProcessStartInfo startInfo)
    {
        var psi = new ProcessStartInfo(startInfo.FileName, startInfo.Arguments)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start the process.");
        var stdOut = process.StandardOutput.ReadToEnd().Trim();
        var stdErr = process.StandardError.ReadToEnd().Trim();
        process.WaitForExit();
        return new GitHubProcessResult(process.ExitCode, stdOut, stdErr);
    }
}

// Default adapter forwards to the system clock.
[ExcludeFromCodeCoverage]
internal sealed class GitHubClock : IGitHubClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public void Sleep(TimeSpan delay) => Thread.Sleep(delay);
}

// Default adapter forwards to clip.exe.
[ExcludeFromCodeCoverage]
internal sealed class GitHubClipboard : IGitHubClipboard
{
    public void SetText(string text)
    {
        var psi = new ProcessStartInfo("cmd.exe", "/c echo|set /p=\"" + text + "\"|clip")
        {
            CreateNoWindow = true,
            UseShellExecute = false
        };
        using var process = Process.Start(psi);
        process?.WaitForExit(2000);
    }
}

// Default adapter forwards to the shell browser association.
[ExcludeFromCodeCoverage]
internal sealed class GitHubBrowserLauncher : IGitHubBrowserLauncher
{
    public void Open(string url)
    {
        var psi = new ProcessStartInfo(url) { UseShellExecute = true };
        Process.Start(psi);
    }
}