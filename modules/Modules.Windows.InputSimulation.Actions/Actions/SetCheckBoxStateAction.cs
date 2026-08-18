// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

[Action(Id = "SetCheckBoxState")]
[Throws(ErrorCodes.AccessDenied)]
[Throws(ErrorCodes.MessageDelivery)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class SetCheckBoxStateAction : InputSimulationActionBase
{
    public SetCheckBoxStateAction()
    {
    }

    internal SetCheckBoxStateAction(InputSimulationContext context) : base(context)
    {
    }

    [InputArgument(Order = 1, Required = true)]
    public WindowObject Control { get; set; } = null!;

    [InputArgument(Order = 2)]
    [DefaultValue(CheckBoxState.Checked)]
    public CheckBoxState State { get; set; } = CheckBoxState.Checked;

    protected override void Run(ActionContext context)
    {
        var handle = RequireHandle(Control, nameof(Control));
        Context.MessageDispatcher.Send(handle, WindowMessages.ButtonSetCheck, (IntPtr)(int)State, IntPtr.Zero);

        // BM_SETCHECK changes the visual state only, so the parent is told separately.
        NotificationSender.NotifyParent(Context, handle, WindowMessages.NotifyButtonClicked);
    }
}
