// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Actions.Shared;
using PowerAutomate.Desktop.Windows.Registry.Abstractions;
using PowerAutomate.Desktop.Windows.Registry.Win32;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

[Action(Id = "GetValuesInRegistryKey")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetValuesInRegistryKeyAction : ActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public string Path { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public List<string> Values { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            using var registry = new Win32Registry();
            using var registryKey = registry.ParseKey(Path, true);
            var registryValues = registryKey.GetValues().ToList();
            var values = new List<string>();

            foreach (var registryValue in registryValues)
            {
                values.Add(registryValue.Name);
                registryValue.Dispose();
            }

            Values = values;
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}