// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions;

[Action(Id = "WhenAny")]
[Throws(ErrorCodes.Unknown)]
public class TasksWhenAnyAction : ActionBase
{
    [InputArgument]
    public List<TaskObject> Tasks { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        try
        {
            var tasks = Tasks.Select(task => task.Task).ToList();
            Task.WhenAny(tasks).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}