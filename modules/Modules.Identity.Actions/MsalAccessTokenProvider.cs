// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Identity.Client;

namespace PowerAutomate.Desktop.Modules.Identity.Actions;

// Thin MSAL adapter excluded from unit coverage.
[ExcludeFromCodeCoverage]
internal sealed class MsalAccessTokenProvider : IAccessTokenProvider
{
    public string GetAccessToken(Uri authority, string clientId, string clientSecret, IReadOnlyCollection<string> scopes)
    {
        var app = ConfidentialClientApplicationBuilder.Create(clientId)
                                                          .WithClientSecret(clientSecret)
                                                          .WithAuthority(authority)
                                                          .Build();
        return app.AcquireTokenForClient(scopes).ExecuteAsync().GetAwaiter().GetResult().AccessToken;
    }
}