// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;

namespace PowerAutomate.Desktop.Modules.Identity.Actions.Tests.Fakes;

internal sealed class FakeAccessTokenProvider : IAccessTokenProvider
{
    public Uri? Authority { get; private set; }
    public string? ClientId { get; private set; }
    public string? ClientSecret { get; private set; }
    public Exception? Exception { get; set; }
    public IReadOnlyCollection<string>? Scopes { get; private set; }
    public string Token { get; set; } = "token";

    public string GetAccessToken(Uri authority, string clientId, string clientSecret, IReadOnlyCollection<string> scopes)
    {
        if (Exception != null)
        {
            throw Exception;
        }

        Authority = authority;
        ClientId = clientId;
        ClientSecret = clientSecret;
        Scopes = scopes;
        return Token;
    }
}