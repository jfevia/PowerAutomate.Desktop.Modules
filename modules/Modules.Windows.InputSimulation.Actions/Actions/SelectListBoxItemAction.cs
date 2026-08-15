// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Extensions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

[Action(Id = "SelectListBoxItem")]
[Throws(ErrorCodes.AccessDenied)]
[Throws(ErrorCodes.ControlNotFound)]
[Throws(ErrorCodes.MessageDelivery)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class SelectListBoxItemAction : ActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public WindowObject Control { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public int? Index { get; set; }

    [OutputArgument(Order = 1)]
    public int SelectedIndex { get; set; }

    [InputArgument(Order = 2, Required = false)]
    public string Text { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            if (Control is null)
            {
                throw new ArgumentException("A control is required to select a list box item.", nameof(Control));
            }

            var handle = Control.NativeHandle;
            var index = ResolveIndex(handle);

            var result = MessageDispatcher.Send(handle, WindowMessages.ListBoxSetCurrentSelection, (IntPtr)index, IntPtr.Zero).ToInt32();
            if (result == WindowMessages.ListBoxError)
            {
                throw new ControlNotFoundException($"index {index} in the list box");
            }

            // LB_SETCURSEL updates the control without telling the parent, so the notification is sent explicitly.
            NotifyParent(handle);
            SelectedIndex = index;
        }
        catch (Exception ex)
        {
            throw ex.ToActionException();
        }
    }

    private int ResolveIndex(IntPtr handle)
    {
        if (Index.HasValue)
        {
            return Index.Value;
        }

        if (string.IsNullOrEmpty(Text))
        {
            throw new ArgumentException("Provide either the item text or the item index to select.", nameof(Text));
        }

        var found = MessageDispatcher.SendText(handle, WindowMessages.ListBoxFindStringExact, (IntPtr)(-1), Text).ToInt32();
        if (found == WindowMessages.ListBoxError)
        {
            throw new ControlNotFoundException($"item '{Text}' in the list box");
        }

        return found;
    }

    private static void NotifyParent(IntPtr handle)
    {
        var parent = NativeMethods.GetParent(handle);
        if (parent == IntPtr.Zero)
        {
            return;
        }

        var controlId = NativeMethods.GetDlgCtrlID(handle);
        var notification = MessageDispatcher.MakeNotification(controlId, WindowMessages.NotifyListBoxSelectionChange);
        MessageDispatcher.Send(parent, WindowMessages.Command, notification, handle);
    }
}
