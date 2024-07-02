// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;
using PowerAutomate.Desktop.Modules.Windows.Registry.Win32;

namespace PowerAutomate.Desktop.Modules.WindowsRegistry.Actions;

[Action]
[Throws(ErrorCodes.Unknown)]
public class SetRegistryValueAction : ActionBase
{
    [InputArgument]
    public string Path { get; set; } = null!;

    [InputArgument]
    public object ValueData { get; set; } = null!;

    [InputArgument]
    public RegistryValueKind ValueKind { get; set; }

    [InputArgument]
    public string ValueName { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            using var registry = new Win32Registry();
            using var registryKey = registry.ParseKey(Path);
            registryKey.SetValue(ValueName, ValueData, ValueKind);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, "An unexpected error occurred", ex);
        }
    }
}