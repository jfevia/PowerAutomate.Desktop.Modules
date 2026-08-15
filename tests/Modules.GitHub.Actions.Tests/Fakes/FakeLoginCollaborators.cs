// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions.Tests.Fakes;

internal sealed class FakeGitHubHttpClientFactory : IGitHubHttpClientFactory
{
    private readonly Queue<HttpMessageHandler> handlers = new();

    public List<HttpClient> Clients { get; } = new();

    public FakeGitHubHttpClientFactory Enqueue(FakeHttpMessageHandler handler)
    {
        handlers.Enqueue(handler);
        return this;
    }

    public HttpClient CreateClient()
    {
        var client = new HttpClient(handlers.Count == 0 ? new FakeHttpMessageHandler() : handlers.Dequeue());
        Clients.Add(client);
        return client;
    }
}

internal sealed class FakeGitHubProcessRunner : IGitHubProcessRunner
{
    public GitHubProcessResult Result { get; set; } = new(0, "token", string.Empty);
    public List<GitHubProcessStartInfo> Starts { get; } = new();

    public GitHubProcessResult Run(GitHubProcessStartInfo startInfo)
    {
        Starts.Add(startInfo);
        return Result;
    }
}

internal sealed class FakeGitHubClock : IGitHubClock
{
    public DateTime UtcNow { get; private set; } = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public List<TimeSpan> Sleeps { get; } = new();

    public void Sleep(TimeSpan delay)
    {
        Sleeps.Add(delay);
        UtcNow = UtcNow.Add(delay);
    }
}

internal sealed class FakeGitHubClipboard : IGitHubClipboard
{
    public bool ThrowOnSet { get; set; }
    public List<string> Values { get; } = new();

    public void SetText(string text)
    {
        if (ThrowOnSet)
        {
            throw new InvalidOperationException("clipboard failed");
        }
        Values.Add(text);
    }
}

internal sealed class FakeGitHubBrowserLauncher : IGitHubBrowserLauncher
{
    public bool ThrowOnOpen { get; set; }
    public List<string> Urls { get; } = new();

    public void Open(string url)
    {
        if (ThrowOnOpen)
        {
            throw new InvalidOperationException("browser failed");
        }
        Urls.Add(url);
    }
}