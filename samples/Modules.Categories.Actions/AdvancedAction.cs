// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Categories.Actions;

[Action]
public class RootAction : ActionBase
{
    public override void Execute(ActionContext context)
    {
        // Nothing
    }
}

[Action(Category = "Advanced")]
public class AdvancedAction : ActionBase
{
    public override void Execute(ActionContext context)
    {
        // Nothing
    }
}