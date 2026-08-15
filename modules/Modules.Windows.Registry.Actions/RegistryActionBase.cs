// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

public abstract class RegistryActionBase : ActionBase
{
    protected RegistryActionBase() : this(RegistryContext.CreateDefault())
    {
    }

    protected RegistryActionBase(RegistryContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected RegistryContext Context { get; }

    public override void Execute(ActionContext context)
    {
        try
        {
            Run(context);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }

    protected abstract void Run(ActionContext context);
}
