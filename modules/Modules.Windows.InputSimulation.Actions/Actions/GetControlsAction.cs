// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

[Action(Id = "GetControls")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetControlsAction : InputSimulationActionBase
{
    public GetControlsAction()
    {
    }

    public GetControlsAction(InputSimulationContext context) : base(context)
    {
    }

    [OutputArgument(Order = 1)]
    public List<WindowObject> Controls { get; set; } = null!;

    [InputArgument(Order = 2)]
    [DefaultValue(true)]
    public bool Recursive { get; set; } = true;

    [InputArgument(Order = 1, Required = true)]
    public WindowObject Window { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var parent = RequireHandle(Window, nameof(Window));
        var handles = Context.WindowService.EnumerateChildWindows(parent, Recursive);
        var controls = new List<WindowObject>(handles.Count);

        foreach (var handle in handles)
        {
            controls.Add(Context.WindowService.ToWindowObject(handle));
        }

        Controls = controls;
    }
}
