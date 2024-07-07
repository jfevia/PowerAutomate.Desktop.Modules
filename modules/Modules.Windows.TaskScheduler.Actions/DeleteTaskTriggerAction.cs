// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Actions.Shared;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

[Action(Id = "DeleteTaskTrigger")]
[Group(Name = "General", Order = 1)]
[Group(Name = "Advanced", Order = 2, IsDefault = true)]
[Throws(ErrorCodes.TaskNotFound)]
[Throws(ErrorCodes.TaskTriggerNotFound)]
[Throws(ErrorCodes.TaskTriggerUnknown)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class DeleteTaskTriggerAction : ActionBase
{
    [InputArgument(Order = 5, Required = false)]
    public string AccountDomain { get; set; } = null!;

    [InputArgument(Order = 6, Required = false)]
    public string Password { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public string TargetServer { get; set; } = null!;

    [InputArgument(Order = 1, Group = "General")]
    public string TaskName { get; set; } = null!;

    [InputArgument(Order = 2, Group = "General")]
    public string TriggerId { get; set; } = null!;

    [InputArgument(Order = 4, Required = false)]
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

            try
            {
                using var trigger = task.Definition.Triggers.FirstOrDefault(trigger => trigger.Id == TriggerId);
                if (trigger is null)
                {
                    throw new TaskTriggerNotFoundException(TaskName, TriggerId);
                }

                if (!task.Definition.Triggers.Remove(trigger))
                {
                    throw new TaskTriggerException(TaskName, TriggerId, $"Could not delete trigger '{TriggerId}' in task '{TaskName}'");
                }
            }
            catch (FileNotFoundException)
            {
                throw new TaskTriggerNotFoundException(TaskName, TriggerId);
            }
        }
        catch (TaskNotFoundException ex)
        {
            throw new ActionException(ErrorCodes.TaskNotFound, ex.Message, ex);
        }
        catch (TaskTriggerNotFoundException ex)
        {
            throw new ActionException(ErrorCodes.TaskTriggerNotFound, ex.Message, ex);
        }
        catch (TaskTriggerException ex)
        {
            throw new ActionException(ErrorCodes.TaskTriggerNotFound, ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}