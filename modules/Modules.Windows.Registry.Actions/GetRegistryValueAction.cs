// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

[Action(Id = "GetRegistryValue")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetRegistryValueAction : RegistryActionBase
{
    public GetRegistryValueAction()
    {
    }

    public GetRegistryValueAction(RegistryContext context) : base(context)
    {
    }

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

    protected override void Run(ActionContext context)
    {
        using var registryKey = Context.RegistryService.OpenKey(Path, true);
        var valueKind = registryKey.GetValueKind(Name);
        Value = registryKey.GetValue(Name, DefaultValue, valueKind.CanExpandEnvironmentVariables() && ExpandEnvironmentVariables)!;
    }
}
