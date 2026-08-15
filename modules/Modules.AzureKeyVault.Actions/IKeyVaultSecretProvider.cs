// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.AzureKeyVault.Actions;

public interface IKeyVaultSecretProvider
{
    string GetSecretValue(Uri vaultUri, string secretName);
}