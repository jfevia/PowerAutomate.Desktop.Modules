// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Extensions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

/// <summary>
/// Shared plumbing so every action resolves its collaborators the same way.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public abstract class InputSimulationActionBase : ActionBase
{
    protected InputSimulationActionBase() : this(InputSimulationContext.CreateDefault())
    {
    }

    protected InputSimulationActionBase(InputSimulationContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
    protected InputSimulationContext Context { get; }

    public override void Execute(ActionContext context)
    {
        try
        {
            Run(context);
        }
        catch (Exception ex)
        {
            throw ex.ToActionException();
        }
    }

    protected abstract void Run(ActionContext context);

    /// <summary>
    /// Guards the handle arguments, which arrive from flow variables and may be unset.
    /// </summary>
    protected static IntPtr RequireHandle(WindowObject value, string argumentName)
    {
        if (value is null)
        {
            throw new ArgumentException("A window or control is required for this action.", argumentName);
        }

        return value.NativeHandle;
    }
}
