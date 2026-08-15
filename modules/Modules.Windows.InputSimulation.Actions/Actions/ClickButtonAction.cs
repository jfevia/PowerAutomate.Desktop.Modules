// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

[Action(Id = "ClickButton")]
[Throws(ErrorCodes.AccessDenied)]
[Throws(ErrorCodes.MessageDelivery)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class ClickButtonAction : InputSimulationActionBase
{
    public ClickButtonAction()
    {
    }

    public ClickButtonAction(InputSimulationContext context) : base(context)
    {
    }

    [InputArgument(Order = 1, Required = true)]
    public WindowObject Control { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var handle = RequireHandle(Control, nameof(Control));

        // BM_CLICK drives the button's own state machine, so no coordinates are involved.
        Context.MessageDispatcher.Send(handle, WindowMessages.ButtonClick, IntPtr.Zero, IntPtr.Zero);
    }
}
