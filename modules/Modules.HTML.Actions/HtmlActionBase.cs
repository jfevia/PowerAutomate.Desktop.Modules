// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;

namespace PowerAutomate.Desktop.Modules.HTML.Actions;

public abstract class HtmlActionBase : ActionBase
{
    protected HtmlActionBase() : this(HtmlActionsContext.CreateDefault())
    {
    }

    protected HtmlActionBase(HtmlActionsContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected HtmlActionsContext Context { get; }

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
