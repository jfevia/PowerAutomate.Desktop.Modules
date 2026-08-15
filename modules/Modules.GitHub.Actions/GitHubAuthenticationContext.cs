// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions;

[Type(FriendlyName = nameof(GitHubAuthenticationContext) + "_FriendlyName",
      FriendlyNamePlural = nameof(GitHubAuthenticationContext) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
public sealed class GitHubAuthenticationContext : IDisposable
{
    public GitHubAuthenticationContext(HttpClient httpClient, string baseUrl, string userAgent, string login)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
        Login = login ?? string.Empty;
    }

    [PropertyIgnore]
    public HttpClient HttpClient { get; }

    [Property]
    public string BaseUrl { get; }

    [Property]
    public string UserAgent { get; }

    [Property]
    public string Login { get; }

    public void Dispose() => HttpClient.Dispose();

    public override string ToString() =>
        string.IsNullOrEmpty(Login) ? BaseUrl : Login + " @ " + BaseUrl;
}
