// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Extensions;
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
public class SetCheckBoxStateAction : ActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public WindowObject Control { get; set; } = null!;

    [InputArgument(Order = 2)]
    [DefaultValue(CheckBoxState.Checked)]
    public CheckBoxState State { get; set; } = CheckBoxState.Checked;

    public override void Execute(ActionContext context)
    {
        try
        {
            if (Control is null)
            {
                throw new ArgumentException("A control is required to set a check box state.", nameof(Control));
            }

            var handle = Control.NativeHandle;
            MessageDispatcher.Send(handle, WindowMessages.ButtonSetCheck, (IntPtr)(int)State, IntPtr.Zero);

            // BM_SETCHECK changes the visual state only, so the parent is told separately.
            NotifyParent(handle);
        }
        catch (Exception ex)
        {
            throw ex.ToActionException();
        }
    }

    private static void NotifyParent(IntPtr handle)
    {
        var parent = NativeMethods.GetParent(handle);
        if (parent == IntPtr.Zero)
        {
            return;
        }

        var controlId = NativeMethods.GetDlgCtrlID(handle);
        var notification = MessageDispatcher.MakeNotification(controlId, WindowMessages.NotifyButtonClicked);
        MessageDispatcher.Send(parent, WindowMessages.Command, notification, handle);
    }
}
