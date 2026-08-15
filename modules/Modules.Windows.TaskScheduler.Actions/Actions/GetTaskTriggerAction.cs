// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Exceptions;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Actions;

[Action(Id = "GetTaskTrigger")]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.TaskNotFound)]
[Throws(ErrorCodes.TaskTriggerNotFound)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetTaskTriggerAction : TaskSchedulerActionBase
{
    [InputArgument(Order = 5, Required = false)]
    public string AccountDomain { get; set; } = null!;

    [InputArgument(Order = 6, Required = false)]
    public string Password { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public string TargetServer { get; set; } = null!;

    [InputArgument(Order = 1, Group = Groups.General)]
    public string TaskName { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public TaskTriggerObject TaskTrigger { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.General)]
    public string TriggerId { get; set; } = null!;

    [InputArgument(Order = 4, Required = false)]
    public string UserName { get; set; } = null!;


    public GetTaskTriggerAction()
    {
    }

    public GetTaskTriggerAction(TaskSchedulerContext context) : base(context)
    {
    }

    protected override void Run(ActionContext context)
    {
        using var taskService = Connect(TargetServer, UserName, AccountDomain, Password);
        using var task = RequireTask(taskService, TaskName);
        using var trigger = RequireTrigger(task, TriggerId);

        TaskTrigger = trigger.ToAction(task.Name);
    }
}