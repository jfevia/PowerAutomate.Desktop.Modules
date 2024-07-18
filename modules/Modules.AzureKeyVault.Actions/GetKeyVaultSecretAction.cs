// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
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
            var client = new SecretClient(new Uri(VaultUrl), new DefaultAzureCredential());
            KeyVaultSecret secret = client.GetSecret(SecretName);

            SecretValue = secret.Value;
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}