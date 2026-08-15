// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Identity.Actions;

[Action(Id = "GetAccessToken")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetAccessTokenAction : ActionBase
{
    private readonly IAccessTokenProvider tokenProvider;

    public GetAccessTokenAction() : this(new MsalAccessTokenProvider())
    {
    }

    public GetAccessTokenAction(IAccessTokenProvider tokenProvider)
    {
        this.tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
    }

    [InputArgument(Order = 3, Required = true)]
    public string Authority { get; set; } = null!;

    [InputArgument(Order = 1, Required = true)]
    public string ClientId { get; set; } = null!;

    [InputArgument(Order = 2, Required = true)]
    public string ClientSecret { get; set; } = null!;

    [InputArgument(Order = 4, Required = true)]
    public List<string> Scopes { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public string Token { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            var clientId = RequireValue(ClientId, nameof(ClientId));
            var clientSecret = RequireValue(ClientSecret, nameof(ClientSecret));
            var authority = new Uri(RequireValue(Authority, nameof(Authority)), UriKind.Absolute);
            var scopes = RequireScopes();
            Token = tokenProvider.GetAccessToken(authority, clientId, clientSecret, scopes);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }

    private IReadOnlyCollection<string> RequireScopes()
    {
        if (Scopes is null || Scopes.Count == 0)
        {
            throw new ArgumentException("At least one scope is required.", nameof(Scopes));
        }

        if (Scopes.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Scopes cannot be empty.", nameof(Scopes));
        }

        return Scopes.ToArray();
    }

    private static string RequireValue(string value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A value is required.", argumentName);
        }

        return value;
    }
}