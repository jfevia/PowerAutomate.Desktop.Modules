// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Newtonsoft.Json.Linq;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions;

[Action(Id = "Login")]
[Throws(ErrorCodes.Unknown)]
[Group(Name = Groups.General,             Order = 1, IsDefault = true)]
[Group(Name = Groups.PersonalAccessToken, Order = 2)]
[Group(Name = Groups.DeviceFlow,          Order = 3)]
[Group(Name = Groups.GitHubCli,           Order = 4)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class LoginAction : ActionBase
{
    private const string DefaultBaseUrl = "https://api.github.com";
    private const string DefaultGitHubCliHost = "github.com";
    private const string GitHubCliOAuthClientId = "178c6fc6a1e3a7d9b8a0";
    private const string DefaultDeviceFlowScopes = "repo,read:org,gist,workflow";
    private const string DeviceCodeUrl = "https://github.com/login/device/code";
    private const string DeviceFlowTokenUrl = "https://github.com/login/oauth/access_token";
    private const string DeviceFlowGrantType = "urn:ietf:params:oauth:grant-type:device_code";

    // -- General ----------------------------------------------------------------
    [InputArgument(Order = 1, Group = Groups.General)]
    [DefaultValue(LoginMode.PersonalAccessToken)]
    public LoginMode Mode { get; set; } = LoginMode.PersonalAccessToken;

    [InputArgument(Order = 2, Required = false, Group = Groups.General)]
    public string BaseUrl { get; set; } = string.Empty;

    [InputArgument(Order = 3, Required = false, Group = Groups.General)]
    public string UserAgent { get; set; } = string.Empty;

    // -- PersonalAccessToken ----------------------------------------------------
    [InputArgument(Order = 4, Required = true, Group = Groups.PersonalAccessToken)]
    public string Token { get; set; } = string.Empty;

    // -- DeviceFlow -------------------------------------------------------------
    [InputArgument(Order = 5, Required = false, Group = Groups.DeviceFlow)]
    public string ClientId { get; set; } = string.Empty;

    [InputArgument(Order = 6, Required = false, Group = Groups.DeviceFlow)]
    public string Scopes { get; set; } = string.Empty;

    // -- GitHubCli --------------------------------------------------------------
    [InputArgument(Order = 7, Required = false, Group = Groups.GitHubCli)]
    public string Host { get; set; } = string.Empty;

    // -- Output -----------------------------------------------------------------
    [OutputArgument(Order = 1)]
    public GitHubAuthenticationContext Authentication { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            switch (Mode)
            {
                case LoginMode.PersonalAccessToken:
                    Authentication = LoginWithPersonalAccessToken();
                    break;
                case LoginMode.DeviceFlow:
                    Authentication = LoginWithDeviceFlow();
                    break;
                case LoginMode.GitHubCli:
                    Authentication = LoginWithGitHubCli();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Mode), Mode, "Unknown login mode.");
            }
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }

    private GitHubAuthenticationContext LoginWithPersonalAccessToken()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            throw new ArgumentException("Token is required for PersonalAccessToken mode.", nameof(Token));
        }
        return CreateAuthenticatedContext(Token);
    }

    private GitHubAuthenticationContext LoginWithDeviceFlow()
    {
        var clientId = string.IsNullOrWhiteSpace(ClientId) ? GitHubCliOAuthClientId : ClientId.Trim();
        var scopes = (string.IsNullOrWhiteSpace(Scopes) ? DefaultDeviceFlowScopes : Scopes).Replace(',', ' ').Trim();

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var deviceCodeResp = http.PostAsync(DeviceCodeUrl, new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("scope", scopes),
        })).GetAwaiter().GetResult();
        deviceCodeResp.EnsureSuccessStatusCode();

        var deviceCodeJson = JObject.Parse(deviceCodeResp.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        var deviceCode = (string)deviceCodeJson["device_code"]!;
        var userCode = (string)deviceCodeJson["user_code"]!;
        var verificationUri = (string)deviceCodeJson["verification_uri"]!;
        var interval = (int?)deviceCodeJson["interval"] ?? 5;
        var expiresIn = (int?)deviceCodeJson["expires_in"] ?? 900;

        CopyTextToClipboard(userCode);
        OpenInDefaultBrowser(verificationUri);
        Console.WriteLine();
        Console.WriteLine("GitHub device flow: open " + verificationUri + " in a browser and enter user code " + userCode + ".");
        Console.WriteLine("The user code has been copied to your clipboard and the verification URL has been opened in your default browser.");
        Console.WriteLine("Waiting for authorization (expires in " + expiresIn + "s)...");

        var deadline = DateTime.UtcNow.AddSeconds(expiresIn);
        while (DateTime.UtcNow < deadline)
        {
            Thread.Sleep(TimeSpan.FromSeconds(interval));

            var tokenResp = http.PostAsync(DeviceFlowTokenUrl, new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("device_code", deviceCode),
                new KeyValuePair<string, string>("grant_type", DeviceFlowGrantType),
            })).GetAwaiter().GetResult();

            var tokenJson = JObject.Parse(tokenResp.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            var accessToken = (string?)tokenJson["access_token"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                return CreateAuthenticatedContext(accessToken!);
            }

            var error = (string?)tokenJson["error"];
            switch (error)
            {
                case "authorization_pending":
                    continue;
                case "slow_down":
                    interval += 5;
                    continue;
                default:
                    throw new InvalidOperationException("Device flow failed: " + (error ?? "unknown error"));
            }
        }

        throw new TimeoutException("Device flow expired without user authorization.");
    }

    private GitHubAuthenticationContext LoginWithGitHubCli()
    {
        var host = string.IsNullOrWhiteSpace(Host) ? DefaultGitHubCliHost : Host.Trim();
        var startInfo = new ProcessStartInfo("gh", "auth token --hostname " + host)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start the gh CLI process.");
        var stdOut = process.StandardOutput.ReadToEnd().Trim();
        var stdErr = process.StandardError.ReadToEnd().Trim();
        process.WaitForExit();

        if (process.ExitCode != 0 || string.IsNullOrEmpty(stdOut))
        {
            throw new InvalidOperationException("gh CLI returned exit code " + process.ExitCode + ". " + stdErr);
        }

        return CreateAuthenticatedContext(stdOut);
    }

    private GitHubAuthenticationContext CreateAuthenticatedContext(string token)
    {
        var baseUrl = (string.IsNullOrWhiteSpace(BaseUrl) ? DefaultBaseUrl : BaseUrl.Trim()).TrimEnd('/');
        var assemblyVersion = GetType().Assembly.GetName().Version?.ToString() ?? "0.0.0.0";
        var userAgent = string.IsNullOrWhiteSpace(UserAgent)
            ? "PowerAutomate.Desktop.Modules.GitHub/" + assemblyVersion
            : UserAgent.Trim();

        var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl + "/") };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

        var login = string.Empty;
        try
        {
            var userResp = httpClient.GetAsync("user").GetAwaiter().GetResult();
            if (userResp.IsSuccessStatusCode)
            {
                var userJson = JObject.Parse(userResp.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                login = (string?)userJson["login"] ?? string.Empty;
            }
        }
        catch
        {
            // Best-effort: leave login empty if /user is unreachable or denied.
        }

        return new GitHubAuthenticationContext(httpClient, baseUrl, userAgent, login);
    }

    private static void CopyTextToClipboard(string text)
    {
        try
        {
            var psi = new ProcessStartInfo("cmd.exe", "/c echo|set /p=\"" + text + "\"|clip")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using var process = Process.Start(psi);
            process?.WaitForExit(2000);
        }
        catch
        {
            // Best-effort: clipboard is a convenience, not a requirement.
        }
    }

    private static void OpenInDefaultBrowser(string url)
    {
        try
        {
            var psi = new ProcessStartInfo(url) { UseShellExecute = true };
            Process.Start(psi);
        }
        catch
        {
            // Best-effort: the user can still navigate to the URL manually.
        }
    }
}
