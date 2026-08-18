// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions;

[Action(Id = "WhenAny")]
[Throws(ErrorCodes.Unknown)]
public class TasksWhenAnyAction : ActionBase
{
    private readonly ITaskRunner taskRunner;

    public TasksWhenAnyAction() : this(new TaskRunner())
    {
    }

    internal TasksWhenAnyAction(ITaskRunner taskRunner)
    {
        this.taskRunner = taskRunner ?? throw new ArgumentNullException(nameof(taskRunner));
    }

    [InputArgument]
    public List<TaskObject> Tasks { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            taskRunner.WhenAny(Tasks).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}