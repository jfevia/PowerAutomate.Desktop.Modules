// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace PowerAutomate.Desktop.Modules.AzureKeyVault.Actions;

// Thin Azure SDK adapter excluded from unit coverage.
[ExcludeFromCodeCoverage]
internal sealed class AzureKeyVaultSecretProvider : IKeyVaultSecretProvider
{
    public string GetSecretValue(Uri vaultUri, string secretName)
    {
        var client = new SecretClient(vaultUri, new DefaultAzureCredential());
        KeyVaultSecret secret = client.GetSecret(secretName);
        return secret.Value;
    }
}