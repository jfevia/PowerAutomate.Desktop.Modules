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
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Extensions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

[Action(Id = "FindWindow")]
[Throws(ErrorCodes.WindowNotFound)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class FindWindowAction : ActionBase
{
    [InputArgument(Order = 2, Required = false)]
    public string ClassName { get; set; } = null!;

    [InputArgument(Order = 3)]
    [DefaultValue(TextMatchMode.Contains)]
    public TextMatchMode MatchMode { get; set; } = TextMatchMode.Contains;

    [InputArgument(Order = 4, Required = false)]
    public int? ProcessId { get; set; }

    [InputArgument(Order = 5)]
    [DefaultValue(0)]
    public int TimeoutMilliseconds { get; set; }

    [InputArgument(Order = 1, Required = false)]
    public string Title { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public WindowObject Window { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            var handle = WindowSearch.FindFirst(
                WindowExtensions.EnumerateTopLevelWindows,
                candidate => WindowSearch.MatchesWindow(candidate, Title, ClassName, ProcessId, MatchMode),
                TimeoutMilliseconds);

            if (handle == IntPtr.Zero)
            {
                throw new WindowNotFoundException(WindowSearch.DescribeCriteria(
                    ("title", Title),
                    ("class", ClassName),
                    ("process", ProcessId)));
            }

            Window = WindowExtensions.ToWindowObject(handle);
        }
        catch (Exception ex)
        {
            throw ex.ToActionException();
        }
    }
}
