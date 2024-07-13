// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions;

[Action(Id = "Delay")]
[Throws(ErrorCodes.Unknown)]
public class TaskDelayAction : ActionBase
{
    [InputArgument(Order = 2)]
    public int DelayInMilliseconds { get; set; }

    [InputArgument(Order = 1)]
    public string Name { get; set; } = null!;

    [OutputArgument(Order = 0)]
    public TaskObject Task { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            var task = System.Threading.Tasks.Task.Delay(DelayInMilliseconds);
            Task = new TaskObject(Name, task);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}