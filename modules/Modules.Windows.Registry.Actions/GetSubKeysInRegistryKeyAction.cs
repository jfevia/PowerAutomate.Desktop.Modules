// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

[Action(Id = "GetSubKeysInRegistryKey")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetSubKeysInRegistryKeyAction : RegistryActionBase
{
    public GetSubKeysInRegistryKeyAction()
    {
    }

    public GetSubKeysInRegistryKeyAction(RegistryContext context) : base(context)
    {
    }

    [InputArgument(Order = 1, Required = true)]
    public string Path { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public List<string> SubKeys { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        using var registryKey = Context.RegistryService.OpenKey(Path, true);
        SubKeys = registryKey.GetSubKeyNames().ToList();
    }
}
