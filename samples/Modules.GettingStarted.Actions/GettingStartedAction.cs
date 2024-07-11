// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.GettingStarted.Actions;

[Action]
public class GettingStartedAction : ActionBase
{
    [OutputArgument]
    public string Message { get; set; } = null!;

    [InputArgument]
    public string Name { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Message = $"Hello, {Name}!";
    }
}