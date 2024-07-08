// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Actions.Shared;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

[Action(Id = "ToggleTaskTrigger")]
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
public class ToggleTaskTriggerAction : ActionBase
{
    [InputArgument(Order = 6, Required = false)]
    public string AccountDomain { get; set; } = null!;

    [InputArgument(Order = 3, Group = Groups.General)]
    [DefaultValue(true)]
    public bool Enabled { get; set; }

    [InputArgument(Order = 7, Required = false)]
    public string Password { get; set; } = null!;

    [InputArgument(Order = 4, Required = false)]
    public string TargetServer { get; set; } = null!;

    [InputArgument(Order = 1, Group = Groups.General)]
    public string TaskName { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.General)]
    public string TriggerId { get; set; } = null!;

    [InputArgument(Order = 5, Required = false)]
    public string UserName { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            using var taskService = new TaskService(TargetServer, UserName, AccountDomain, Password);
            using var task = taskService.FindTask(TaskName);
            if (task is null)
            {
                throw new TaskNotFoundException(TaskName);
            }

            using var trigger = task.Definition.Triggers.FirstOrDefault(trigger => trigger.Id == TriggerId);
            if (trigger is null)
            {
                throw new TaskTriggerNotFoundException(TaskName, TriggerId);
            }

            trigger.Enabled = Enabled;
        }
        catch (TaskNotFoundException ex)
        {
            throw new ActionException(ErrorCodes.TaskNotFound, ex.Message, ex);
        }
        catch (TaskTriggerNotFoundException ex)
        {
            throw new ActionException(ErrorCodes.TaskTriggerNotFound, ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}