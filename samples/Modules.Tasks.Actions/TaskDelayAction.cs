// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions;

[Action(Id = "Delay")]
[Throws(ErrorCodes.Unknown)]
public class TaskDelayAction : ActionBase
{
    private readonly ITaskRunner taskRunner;

    public TaskDelayAction() : this(new TaskRunner())
    {
    }

    public TaskDelayAction(ITaskRunner taskRunner)
    {
        this.taskRunner = taskRunner ?? throw new ArgumentNullException(nameof(taskRunner));
    }

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
            Task = new TaskObject(Name, taskRunner.Delay(DelayInMilliseconds));
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}