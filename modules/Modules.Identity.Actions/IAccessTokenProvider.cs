// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;

namespace PowerAutomate.Desktop.Modules.Identity.Actions;

public interface IAccessTokenProvider
{
    string GetAccessToken(Uri authority, string clientId, string clientSecret, IReadOnlyCollection<string> scopes);
}