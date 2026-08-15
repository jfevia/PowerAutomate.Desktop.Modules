// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

[Action(Id = "SelectComboBoxItem")]
[Throws(ErrorCodes.AccessDenied)]
[Throws(ErrorCodes.ControlNotFound)]
[Throws(ErrorCodes.MessageDelivery)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class SelectComboBoxItemAction : InputSimulationActionBase
{
    public SelectComboBoxItemAction()
    {
    }

    public SelectComboBoxItemAction(InputSimulationContext context) : base(context)
    {
    }

    [InputArgument(Order = 1, Required = true)]
    public WindowObject Control { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public int? Index { get; set; }

    [OutputArgument(Order = 1)]
    public int SelectedIndex { get; set; }

    [InputArgument(Order = 2, Required = false)]
    public string Text { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var handle = RequireHandle(Control, nameof(Control));
        var index = ListSelection.Resolve(
            Context,
            handle,
            Index,
            Text,
            WindowMessages.ComboBoxFindStringExact,
            WindowMessages.ComboBoxSetCurrentSelection,
            WindowMessages.ComboBoxError,
            "combo box");

        // CB_SETCURSEL updates the control without telling the parent, so the notification is sent explicitly.
        NotificationSender.NotifyParent(Context, handle, WindowMessages.NotifyComboBoxSelectionChange);
        SelectedIndex = index;
    }
}
