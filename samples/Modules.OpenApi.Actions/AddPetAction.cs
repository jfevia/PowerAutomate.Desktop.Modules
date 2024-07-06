// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Net.Http;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.PetStore.Client;

namespace PowerAutomate.Desktop.Modules.OpenApi.Actions;

[Action(Id = "Run")]
[Throws(ErrorCodes.Unknown)]
public class AddPetAction : ActionBase
{
    private readonly HttpClient _httpClient;

    public AddPetAction(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public override void Execute(ActionContext context)
    {
        try
        {
            var client = new PetStoreClient(_httpClient);
            client.AddPetAsync(new Pet()).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}