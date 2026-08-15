// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Net;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.GitHub.Actions;
using PowerAutomate.Desktop.Modules.GitHub.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions.Tests;

[TestFixture]
public class LoginActionTests
{
    [Test]
    public void Constructor_RequiresCollaborators()
    {
        var collaborators = CreateCollaborators();

        Assert.Throws<ArgumentNullException>(() => new LoginAction(null!, collaborators.ProcessRunner, collaborators.Clock, collaborators.Clipboard, collaborators.Browser));
        Assert.Throws<ArgumentNullException>(() => new LoginAction(collaborators.Factory, null!, collaborators.Clock, collaborators.Clipboard, collaborators.Browser));
        Assert.Throws<ArgumentNullException>(() => new LoginAction(collaborators.Factory, collaborators.ProcessRunner, null!, collaborators.Clipboard, collaborators.Browser));
        Assert.Throws<ArgumentNullException>(() => new LoginAction(collaborators.Factory, collaborators.ProcessRunner, collaborators.Clock, null!, collaborators.Browser));
        Assert.Throws<ArgumentNullException>(() => new LoginAction(collaborators.Factory, collaborators.ProcessRunner, collaborators.Clock, collaborators.Clipboard, null!));
    }

    [Test]
    public void ParameterlessConstructor_IsAvailable()
    {
        Assert.That(new LoginAction(), Is.Not.Null);
    }

    [Test]
    public void PersonalAccessToken_UsesDefaultsAndReadsLogin()
    {
        var collaborators = CreateCollaborators(new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, "{\"login\":\"octo\"}"));
        var action = CreateAction(collaborators); action.Token = " token ";

        action.Execute(new ActionContext());

        Assert.That(action.Authentication.BaseUrl, Is.EqualTo("https://api.github.com"));
        Assert.That(action.Authentication.Login, Is.EqualTo("octo"));
        Assert.That(action.Authentication.HttpClient.DefaultRequestHeaders.Authorization!.Parameter, Is.EqualTo(" token "));
    }

    [Test]
    public void PersonalAccessToken_UsesCustomBaseUrlAndUserAgent()
    {
        var collaborators = CreateCollaborators(new FakeHttpMessageHandler().Enqueue(HttpStatusCode.NotFound, "{}"));
        var action = CreateAction(collaborators); action.Token = "token"; action.BaseUrl = " https://github.test/api/ "; action.UserAgent = " agent ";

        action.Execute(new ActionContext());

        Assert.That(action.Authentication.BaseUrl, Is.EqualTo("https://github.test/api"));
        Assert.That(action.Authentication.UserAgent, Is.EqualTo("agent"));
        Assert.That(action.Authentication.Login, Is.Empty);
    }

    [Test]
    public void PersonalAccessToken_UserLookupFailureLeavesLoginEmpty()
    {
        var collaborators = CreateCollaborators(new FakeHttpMessageHandler().EnqueueException(new InvalidOperationException("boom")));
        var action = CreateAction(collaborators); action.Token = "token";

        action.Execute(new ActionContext());

        Assert.That(action.Authentication.Login, Is.Empty);
    }

    [Test]
    public void PersonalAccessToken_RequiresToken()
    {
        var action = CreateAction(CreateCollaborators());

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));

        Assert.That(exception!.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void Execute_RejectsUnknownMode()
    {
        var action = CreateAction(CreateCollaborators()); action.Mode = (LoginMode)999;

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));

        Assert.That(exception!.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void GitHubCli_UsesDefaultHostAndToken()
    {
        var collaborators = CreateCollaborators(new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, "{\"login\":\"cli\"}"));
        var action = CreateAction(collaborators); action.Mode = LoginMode.GitHubCli;

        action.Execute(new ActionContext());

        Assert.That(collaborators.ProcessRunner.Starts[0].Arguments, Is.EqualTo("auth token --hostname github.com"));
        Assert.That(action.Authentication.Login, Is.EqualTo("cli"));
    }

    [Test]
    public void GitHubCli_UsesCustomHost()
    {
        var collaborators = CreateCollaborators(new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, "{}"));
        var action = CreateAction(collaborators); action.Mode = LoginMode.GitHubCli; action.Host = " ghe.test ";

        action.Execute(new ActionContext());

        Assert.That(collaborators.ProcessRunner.Starts[0].Arguments, Is.EqualTo("auth token --hostname ghe.test"));
    }

    [TestCase(1, "", "denied")]
    [TestCase(0, "", "")]
    public void GitHubCli_RejectsFailedOrEmptyOutput(int exitCode, string output, string error)
    {
        var collaborators = CreateCollaborators();
        collaborators.ProcessRunner.Result = new GitHubProcessResult(exitCode, output, error);
        var action = CreateAction(collaborators); action.Mode = LoginMode.GitHubCli;

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));

        Assert.That(exception!.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DeviceFlow_CompletesAfterPendingAndSlowDown()
    {
        var flow = new FakeHttpMessageHandler()
            .Enqueue(HttpStatusCode.OK, "{\"device_code\":\"dev\",\"user_code\":\"user\",\"verification_uri\":\"https://verify\",\"interval\":1,\"expires_in\":20}")
            .Enqueue(HttpStatusCode.OK, "{\"error\":\"authorization_pending\"}")
            .Enqueue(HttpStatusCode.OK, "{\"error\":\"slow_down\"}")
            .Enqueue(HttpStatusCode.OK, "{\"access_token\":\"device-token\"}");
        var user = new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, "{\"login\":\"device\"}");
        var collaborators = CreateCollaborators(flow, user);
        var action = CreateAction(collaborators); action.Mode = LoginMode.DeviceFlow; action.ClientId = " client "; action.Scopes = " repo,gist ";

        action.Execute(new ActionContext());

        Assert.That(collaborators.Clipboard.Values, Is.EqualTo(new[] { "user" }));
        Assert.That(collaborators.Browser.Urls, Is.EqualTo(new[] { "https://verify" }));
        Assert.That(collaborators.Clock.Sleeps, Is.EqualTo(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(6) }));
        Assert.That(action.Authentication.Login, Is.EqualTo("device"));
    }

    [Test]
    public void DeviceFlow_UsesDefaultsAndIgnoresClipboardAndBrowserFailures()
    {
        var flow = new FakeHttpMessageHandler()
            .Enqueue(HttpStatusCode.OK, "{\"device_code\":\"dev\",\"user_code\":\"user\",\"verification_uri\":\"https://verify\"}")
            .Enqueue(HttpStatusCode.OK, "{\"access_token\":\"device-token\"}");
        var collaborators = CreateCollaborators(flow, new FakeHttpMessageHandler().Enqueue(HttpStatusCode.OK, "{}"));
        collaborators.Clipboard.ThrowOnSet = true;
        collaborators.Browser.ThrowOnOpen = true;
        var action = CreateAction(collaborators); action.Mode = LoginMode.DeviceFlow;

        action.Execute(new ActionContext());

        Assert.That(collaborators.Clock.Sleeps, Is.EqualTo(new[] { TimeSpan.FromSeconds(5) }));
    }

    [Test]
    public void DeviceFlow_WrapsDeviceCodeHttpFailure()
    {
        var collaborators = CreateCollaborators(new FakeHttpMessageHandler().Enqueue(HttpStatusCode.InternalServerError, "{}"));
        var action = CreateAction(collaborators); action.Mode = LoginMode.DeviceFlow;

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));

        Assert.That(exception!.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [TestCase("bad")]
    [TestCase(null)]
    public void DeviceFlow_WrapsTerminalErrors(string? error)
    {
        var tokenBody = error is null ? "{}" : "{\"error\":\"" + error + "\"}";
        var flow = new FakeHttpMessageHandler()
            .Enqueue(HttpStatusCode.OK, "{\"device_code\":\"dev\",\"user_code\":\"user\",\"verification_uri\":\"https://verify\",\"expires_in\":10}")
            .Enqueue(HttpStatusCode.OK, tokenBody);
        var collaborators = CreateCollaborators(flow);
        var action = CreateAction(collaborators); action.Mode = LoginMode.DeviceFlow;

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));

        Assert.That(exception!.Name, Is.EqualTo(ErrorCodes.Unknown));
    }

    [Test]
    public void DeviceFlow_TimesOutWhenExpiredBeforePolling()
    {
        var flow = new FakeHttpMessageHandler()
            .Enqueue(HttpStatusCode.OK, "{\"device_code\":\"dev\",\"user_code\":\"user\",\"verification_uri\":\"https://verify\",\"expires_in\":0}");
        var collaborators = CreateCollaborators(flow);
        var action = CreateAction(collaborators); action.Mode = LoginMode.DeviceFlow;

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));

        Assert.That(exception!.Name, Is.EqualTo(ErrorCodes.Unknown));
        Assert.That(collaborators.Clock.Sleeps, Is.Empty);
    }

    private static LoginAction CreateAction(Collaborators collaborators) => new(collaborators.Factory, collaborators.ProcessRunner, collaborators.Clock, collaborators.Clipboard, collaborators.Browser);

    private static Collaborators CreateCollaborators(params FakeHttpMessageHandler[] handlers)
    {
        var factory = new FakeGitHubHttpClientFactory();
        foreach (var handler in handlers)
        {
            factory.Enqueue(handler);
        }
        return new Collaborators(factory, new FakeGitHubProcessRunner(), new FakeGitHubClock(), new FakeGitHubClipboard(), new FakeGitHubBrowserLauncher());
    }

        private sealed class Collaborators
    {
        public Collaborators(FakeGitHubHttpClientFactory factory, FakeGitHubProcessRunner processRunner, FakeGitHubClock clock, FakeGitHubClipboard clipboard, FakeGitHubBrowserLauncher browser)
        {
            Factory = factory;
            ProcessRunner = processRunner;
            Clock = clock;
            Clipboard = clipboard;
            Browser = browser;
        }

        public FakeGitHubHttpClientFactory Factory { get; }
        public FakeGitHubProcessRunner ProcessRunner { get; }
        public FakeGitHubClock Clock { get; }
        public FakeGitHubClipboard Clipboard { get; }
        public FakeGitHubBrowserLauncher Browser { get; }
    }
}