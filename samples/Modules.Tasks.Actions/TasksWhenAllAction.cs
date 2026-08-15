// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions;

[Action(Id = "WhenAll")]
[Throws(ErrorCodes.Unknown)]
public class TasksWhenAllAction : ActionBase
{
    private readonly ITaskRunner taskRunner;

    public TasksWhenAllAction() : this(new TaskRunner())
    {
    }

    public TasksWhenAllAction(ITaskRunner taskRunner)
    {
        this.taskRunner = taskRunner ?? throw new ArgumentNullException(nameof(taskRunner));
    }

    [InputArgument]
    public List<TaskObject> Tasks { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            taskRunner.WhenAll(Tasks).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}