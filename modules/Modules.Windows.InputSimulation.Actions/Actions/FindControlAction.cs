// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Services;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

[Action(Id = "FindControl")]
[Throws(ErrorCodes.ControlNotFound)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class FindControlAction : InputSimulationActionBase
{
    public FindControlAction()
    {
    }

    public FindControlAction(InputSimulationContext context) : base(context)
    {
    }

    [InputArgument(Order = 3, Required = false)]
    public string ClassName { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public WindowObject Control { get; set; } = null!;

    [InputArgument(Order = 4, Required = false)]
    public int? ControlId { get; set; }

    [InputArgument(Order = 5)]
    [DefaultValue(TextMatchMode.Contains)]
    public TextMatchMode MatchMode { get; set; } = TextMatchMode.Contains;

    [InputArgument(Order = 6)]
    [DefaultValue(true)]
    public bool Recursive { get; set; } = true;

    [InputArgument(Order = 2, Required = false)]
    public string Text { get; set; } = null!;

    [InputArgument(Order = 7)]
    [DefaultValue(0)]
    public int TimeoutMilliseconds { get; set; }

    [InputArgument(Order = 1, Required = true)]
    public WindowObject Window { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var parent = RequireHandle(Window, nameof(Window));
        var handle = Context.WindowService.FindControl(parent, Text, ClassName, ControlId, MatchMode, Recursive, TimeoutMilliseconds);
        if (handle == IntPtr.Zero)
        {
            throw new ControlNotFoundException(Context.WindowService.DescribeCriteria(
                new SearchCriterion("text", Text),
                new SearchCriterion("class", ClassName),
                new SearchCriterion("id", ControlId)));
        }

        Control = Context.WindowService.ToWindowObject(handle);
    }
}
