// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions;

public abstract class NotificationsActionBase : ActionBase
{
    protected NotificationsActionBase() : this(NotificationsContext.CreateDefault())
    {
    }

    protected NotificationsActionBase(NotificationsContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected NotificationsContext Context { get; }

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
