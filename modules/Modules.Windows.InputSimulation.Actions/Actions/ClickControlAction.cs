// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

[Action(Id = "ClickControl")]
[Throws(ErrorCodes.AccessDenied)]
[Throws(ErrorCodes.MessageDelivery)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class ClickControlAction : InputSimulationActionBase
{
    public ClickControlAction()
    {
    }

    public ClickControlAction(InputSimulationContext context) : base(context)
    {
    }

    [InputArgument(Order = 2)]
    [DefaultValue(MouseButton.Left)]
    public MouseButton Button { get; set; } = MouseButton.Left;

    [InputArgument(Order = 1, Required = true)]
    public WindowObject Control { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public int? X { get; set; }

    [InputArgument(Order = 4, Required = false)]
    public int? Y { get; set; }

    protected override void Run(ActionContext context)
    {
        var handle = RequireHandle(Control, nameof(Control));
        Context.WindowService.GetCenter(handle, out var centerX, out var centerY);
        Context.InputSender.Click(handle, Button, X ?? centerX, Y ?? centerY, false);
    }
}
