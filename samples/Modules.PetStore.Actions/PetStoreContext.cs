// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using PowerAutomate.Desktop.PetStore.Client;

namespace PowerAutomate.Desktop.Modules.PetStore.Actions;

public interface IPetStoreClientFactory
{
    IPetStoreClient CreateClient();
}

public sealed class PetStoreContext
{
    public PetStoreContext(IPetStoreClientFactory clientFactory)
    {
        ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    }

    public IPetStoreClientFactory ClientFactory { get; }

    public static PetStoreContext CreateDefault() => new(new PetStoreClientFactory());
}

// Default adapter creates the generated HTTP client.
[ExcludeFromCodeCoverage]
internal sealed class PetStoreClientFactory : IPetStoreClientFactory
{
    public IPetStoreClient CreateClient()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("https://petstore3.swagger.io/api/v3/") };
        return new PetStoreClient(httpClient);
    }
}