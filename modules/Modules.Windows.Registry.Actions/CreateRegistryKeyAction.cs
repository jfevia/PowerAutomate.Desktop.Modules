﻿// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Windows.Registry.Abstractions;
using PowerAutomate.Desktop.Windows.Registry.Win32;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

[Action(Id = "CreateRegistryKey")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class CreateRegistryKeyAction : ActionBase
{
    [InputArgument(Order = 2, Required = true)]
    public string Name { get; set; } = null!;

    [InputArgument(Order = 1, Required = true)]
    public string Path { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            using var registry = new Win32Registry();
            using var registryKey = registry.ParseKey(Path, true);
            registryKey.CreateSubKey(Name);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}