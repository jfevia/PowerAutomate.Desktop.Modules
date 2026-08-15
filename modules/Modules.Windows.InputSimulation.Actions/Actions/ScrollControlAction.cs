// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

[Action(Id = "ScrollControl")]
[Throws(ErrorCodes.AccessDenied)]
[Throws(ErrorCodes.MessageDelivery)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class ScrollControlAction : InputSimulationActionBase
{
    public ScrollControlAction()
    {
    }

    public ScrollControlAction(InputSimulationContext context) : base(context)
    {
    }

    [InputArgument(Order = 1, Required = true)]
    public WindowObject Control { get; set; } = null!;

    [InputArgument(Order = 2)]
    [DefaultValue(ScrollDirection.Down)]
    public ScrollDirection Direction { get; set; } = ScrollDirection.Down;

    [InputArgument(Order = 3)]
    [DefaultValue(3)]
    public int Notches { get; set; } = 3;

    protected override void Run(ActionContext context)
    {
        var handle = RequireHandle(Control, nameof(Control));

        if (Notches <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Notches), Notches, "The number of notches must be greater than zero.");
        }

        Context.WindowService.GetCenter(handle, out var centerX, out var centerY);
        Context.WindowService.ClientToScreen(handle, centerX, centerY, out var screenX, out var screenY);
        Context.InputSender.Scroll(handle, Direction, Notches, screenX, screenY);
    }
}
