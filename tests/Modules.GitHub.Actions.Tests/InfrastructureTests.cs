// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Net.Http;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.GitHub.Actions;
using PowerAutomate.Desktop.Modules.GitHub.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions.Tests;

[TestFixture]
public class InfrastructureTests
{
    [Test]
    public void AuthenticationContext_StoresValuesAndFormatsLogin()
    {
        using var context = new GitHubAuthenticationContext(new HttpClient(), "https://api", "agent", "octo");

        Assert.That(context.HttpClient, Is.Not.Null);
        Assert.That(context.BaseUrl, Is.EqualTo("https://api"));
        Assert.That(context.UserAgent, Is.EqualTo("agent"));
        Assert.That(context.Login, Is.EqualTo("octo"));
        Assert.That(context.ToString(), Is.EqualTo("octo @ https://api"));
    }

    [Test]
    public void AuthenticationContext_NullLoginBecomesEmpty()
    {
        using var context = new GitHubAuthenticationContext(new HttpClient(), "https://api", "agent", null!);

        Assert.That(context.Login, Is.Empty);
    }

    [Test]
    public void GeneratedActionHelpers_EscapesNullAndValues()
    {
        Assert.That(GeneratedActionHelpers.Escape(null), Is.Empty);
        Assert.That(GeneratedActionHelpers.Escape("a b"), Is.EqualTo("a%20b"));
    }

    [Test]
    public void AuthenticationContext_ToStringFallsBackToBaseUrl()
    {
        using var context = new GitHubAuthenticationContext(new HttpClient(), "https://api", "agent", string.Empty);

        Assert.That(context.ToString(), Is.EqualTo("https://api"));
    }

    [Test]
    public void AuthenticationContext_RequiresValues()
    {
        Assert.Throws<ArgumentNullException>(() => new GitHubAuthenticationContext(null!, "https://api", "agent", "octo"));
        Assert.Throws<ArgumentNullException>(() => new GitHubAuthenticationContext(new HttpClient(), null!, "agent", "octo"));
        Assert.Throws<ArgumentNullException>(() => new GitHubAuthenticationContext(new HttpClient(), "https://api", null!, "octo"));
    }

    [Test]
    public void ProcessStartInfo_StoresValues()
    {
        var startInfo = new GitHubProcessStartInfo("gh", "auth token");

        Assert.That(startInfo.FileName, Is.EqualTo("gh"));
        Assert.That(startInfo.Arguments, Is.EqualTo("auth token"));
    }

    [Test]
    public void ProcessStartInfo_RequiresValues()
    {
        Assert.Throws<ArgumentNullException>(() => new GitHubProcessStartInfo(null!, "args"));
        Assert.Throws<ArgumentNullException>(() => new GitHubProcessStartInfo("gh", null!));
    }

    [Test]
    public void ProcessResult_StoresValues()
    {
        var result = new GitHubProcessResult(2, null!, null!);

        Assert.That(result.ExitCode, Is.EqualTo(2));
        Assert.That(result.StandardOutput, Is.Empty);
        Assert.That(result.StandardError, Is.Empty);
    }

    [Test]
    public void ActionSelectors_CanBeConstructed()
    {
        Assert.That(new LoginWithPersonalAccessTokenActionSelector(), Is.Not.Null);
        Assert.That(new LoginWithDeviceFlowActionSelector(), Is.Not.Null);
        Assert.That(new LoginWithGitHubCliActionSelector(), Is.Not.Null);
    }
}