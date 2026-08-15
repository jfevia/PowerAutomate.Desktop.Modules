// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.AzureKeyVault.Actions;

[Action(Id = "GetKeyVaultSecret")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetKeyVaultSecretAction : ActionBase
{
    private readonly IKeyVaultSecretProvider secretProvider;

    public GetKeyVaultSecretAction() : this(new AzureKeyVaultSecretProvider())
    {
    }

    public GetKeyVaultSecretAction(IKeyVaultSecretProvider secretProvider)
    {
        this.secretProvider = secretProvider ?? throw new ArgumentNullException(nameof(secretProvider));
    }

    [InputArgument(Order = 2, Required = true)]
    public string SecretName { get; set; } = null!;

    [OutputArgument]
    public string SecretValue { get; set; } = null!;

    [InputArgument(Order = 1, Required = true)]
    public string VaultUrl { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            var vaultUri = CreateVaultUri(VaultUrl);
            var secretName = RequireValue(SecretName, nameof(SecretName));
            SecretValue = secretProvider.GetSecretValue(vaultUri, secretName);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }

    private static Uri CreateVaultUri(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A vault URL is required.", nameof(VaultUrl));
        }

        return new Uri(value, UriKind.Absolute);
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