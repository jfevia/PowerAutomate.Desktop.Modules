// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.Win32;
using PowerAutomate.Desktop.Modules.Actions.Shared;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

[Action(Id = "GetRegistryValue")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetRegistryValueAction : ActionBase
{
    [InputArgument(Order = 3, Required = false)]
    public object DefaultValue { get; set; } = null!;

    [InputArgument(Order = 4)]
    [DefaultValue(false)]
    public bool ExpandEnvironmentVariables { get; set; }

    [InputArgument(Order = 2, Required = true)]
    public string Name { get; set; } = null!;

    [InputArgument(Order = 1, Required = true)]
    public string Path { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public object Value { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            using var registryKey = RegistryExtensions.ParseKey(Path, true);
            var valueKind = registryKey.GetValueKind(Name)
                                       .ToNative();

            Value = valueKind.CanExpandEnvironmentVariables() && ExpandEnvironmentVariables
                ? registryKey.GetValue(Name, DefaultValue)
                : registryKey.GetValue(Name, DefaultValue, RegistryValueOptions.DoNotExpandEnvironmentNames);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}