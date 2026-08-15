// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.AzureKeyVault.Actions.Tests.Fakes;

internal sealed class FakeKeyVaultSecretProvider : IKeyVaultSecretProvider
{
    public Exception? Exception { get; set; }
    public string SecretValue { get; set; } = "secret-value";
    public string? SecretName { get; private set; }
    public Uri? VaultUri { get; private set; }

    public string GetSecretValue(Uri vaultUri, string secretName)
    {
        if (Exception != null)
        {
            throw Exception;
        }

        VaultUri = vaultUri;
        SecretName = secretName;
        return SecretValue;
    }
}