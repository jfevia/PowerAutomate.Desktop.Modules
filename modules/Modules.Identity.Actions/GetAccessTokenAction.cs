// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Identity.Client;
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
            var app = ConfidentialClientApplicationBuilder.Create(ClientId)
                                                          .WithClientSecret(ClientSecret)
                                                          .WithAuthority(new Uri(Authority))
                                                          .Build();

            var clientParameterBuilder = app.AcquireTokenForClient(Scopes);
            var authenticationResult = clientParameterBuilder.ExecuteAsync().GetAwaiter().GetResult();
            Token = authenticationResult.AccessToken;
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}