// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;

namespace PowerAutomate.Desktop.Modules.PowerFx.Actions;

public abstract class PowerFxActionBase : ActionBase
{
    protected PowerFxActionBase() : this(PowerFxContext.CreateDefault())
    {
    }

    protected PowerFxActionBase(PowerFxContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected PowerFxContext Context { get; }

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
