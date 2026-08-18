// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions;

/// <summary>
/// Base for every action in this module; funnels failures through a single translation point.
/// </summary>
public abstract class OpenTibiaActionBase : ActionBase
{
    public override void Execute(ActionContext context)
    {
        try
        {
            Run(context);
        }
        catch (Exception exception)
        {
            throw ActionErrors.Translate(exception);
        }
    }

    protected abstract void Run(ActionContext context);
}
