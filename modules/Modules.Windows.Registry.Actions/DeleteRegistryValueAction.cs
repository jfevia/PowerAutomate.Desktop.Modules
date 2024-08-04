// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

[Action(Id = "DeleteRegistryValue")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class DeleteRegistryValueAction : ActionBase
{
    [InputArgument(Order = 2, Required = true)]
    public string Name { get; set; } = null!;

    [InputArgument(Order = 1, Required = true)]
    public string Path { get; set; } = null!;

    [InputArgument(Order = 3)]
    [DefaultValue(false)]
    public bool ThrowOnMissingValue { get; set; }

    public override void Execute(ActionContext context)
    {
        try
        {
            using var registryKey = RegistryExtensions.ParseKey(Path, true);
            registryKey.DeleteValue(Name, ThrowOnMissingValue);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}