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

[Action(Id = "SendKey")]
[Throws(ErrorCodes.AccessDenied)]
[Throws(ErrorCodes.MessageDelivery)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class SendKeyAction : ActionBase
{
    [InputArgument(Order = 1, Required = true)]
    public WindowObject Control { get; set; } = null!;

    [InputArgument(Order = 2)]
    [DefaultValue(VirtualKey.Enter)]
    public VirtualKey Key { get; set; } = VirtualKey.Enter;

    public override void Execute(ActionContext context)
    {
        try
        {
            if (Control is null)
            {
                throw new ArgumentException("A control is required to send a key.", nameof(Control));
            }

            InputSender.SendKey(Control.NativeHandle, Key);
        }
        catch (Exception ex)
        {
            throw ex.ToActionException();
        }
    }
}
