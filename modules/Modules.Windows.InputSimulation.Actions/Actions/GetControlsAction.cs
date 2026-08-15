// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Extensions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

[Action(Id = "GetControls")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetControlsAction : ActionBase
{
    [OutputArgument(Order = 1)]
    public List<WindowObject> Controls { get; set; } = null!;

    [InputArgument(Order = 2)]
    [DefaultValue(true)]
    public bool Recursive { get; set; } = true;

    [InputArgument(Order = 1, Required = true)]
    public WindowObject Window { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            if (Window is null)
            {
                throw new ArgumentException("A window is required to enumerate controls.", nameof(Window));
            }

            var handles = WindowExtensions.EnumerateChildWindows(Window.NativeHandle, Recursive);
            var controls = new List<WindowObject>(handles.Count);

            foreach (var handle in handles)
            {
                controls.Add(WindowExtensions.ToWindowObject(handle));
            }

            Controls = controls;
        }
        catch (Exception ex)
        {
            throw ex.ToActionException();
        }
    }
}
