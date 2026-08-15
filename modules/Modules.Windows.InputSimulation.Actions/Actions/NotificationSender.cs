// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

/// <summary>
/// Several control messages change state without telling the parent, so the notification is sent by hand.
/// </summary>
internal static class NotificationSender
{
    public static void NotifyParent(InputSimulationContext context, IntPtr handle, int notificationCode)
    {
        var parent = context.WindowService.GetParent(handle);
        if (parent == IntPtr.Zero)
        {
            return;
        }

        var controlId = context.WindowService.GetControlId(handle);
        var notification = MessageDispatcher.MakeNotification(controlId, notificationCode);
        context.MessageDispatcher.Send(parent, WindowMessages.Command, notification, handle);
    }
}
