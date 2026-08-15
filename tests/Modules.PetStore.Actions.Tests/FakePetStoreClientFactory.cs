// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.PetStore.Client;

namespace PowerAutomate.Desktop.Modules.PetStore.Actions.Tests;

internal sealed class FakePetStoreClientFactory : IPetStoreClientFactory
{
    public FakePetStoreClient Client { get; } = new();

    public IPetStoreClient CreateClient() => Client;
}