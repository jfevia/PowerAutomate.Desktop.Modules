// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Exceptions;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Actions;

[Action(Id = "GetTaskTriggers")]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.TaskNotFound)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetTaskTriggersAction : TaskSchedulerActionBase
{
    [InputArgument(Order = 4, Required = false)]
    public string AccountDomain { get; set; } = null!;

    [InputArgument(Order = 5, Required = false)]
    public string Password { get; set; } = null!;

    [InputArgument(Order = 2, Required = false)]
    public string TargetServer { get; set; } = null!;

    [InputArgument(Order = 1, Group = Groups.General)]
    public string TaskName { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public List<string> Triggers { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public string UserName { get; set; } = null!;


    public GetTaskTriggersAction()
    {
    }

    public GetTaskTriggersAction(TaskSchedulerContext context) : base(context)
    {
    }

    protected override void Run(ActionContext context)
    {
        using var taskService = Connect(TargetServer, UserName, AccountDomain, Password);
        using var task = RequireTask(taskService, TaskName);
        using var triggerCollection = task.Definition.Triggers;
        var triggers = new List<string>();

        foreach (var trigger in triggerCollection)
        {
            triggers.Add(trigger.Id);
            trigger.Dispose();
        }

        Triggers = triggers;
    }
}