﻿// --------------------------------------------------------------
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

[Action(Id = "DeleteTaskAction")]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.TaskNotFound)]
[Throws(ErrorCodes.TaskActionNotFound)]
[Throws(ErrorCodes.TaskActionUnknown)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class DeleteTaskActionAction : ActionBase
{
    [InputArgument(Order = 5, Required = false)]
    public string AccountDomain { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.General)]
    public string ActionId { get; set; } = null!;

    [InputArgument(Order = 6, Required = false)]
    public string Password { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public string TargetServer { get; set; } = null!;

    [InputArgument(Order = 1, Group = Groups.General)]
    public string TaskName { get; set; } = null!;

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
                using var action = task.Definition.Actions.FirstOrDefault(action => action.Id == ActionId);
                if (action is null)
                {
                    throw new TaskActionNotFoundException(TaskName, ActionId);
                }

                if (!task.Definition.Actions.Remove(action))
                {
                    throw new TaskActionException(TaskName, ActionId, $"Could not delete action '{ActionId}' in task '{TaskName}'");
                }
            }
            catch (FileNotFoundException)
            {
                throw new TaskActionNotFoundException(TaskName, ActionId);
            }
        }
        catch (TaskNotFoundException ex)
        {
            throw new ActionException(ErrorCodes.TaskNotFound, ex.Message, ex);
        }
        catch (TaskActionNotFoundException ex)
        {
            throw new ActionException(ErrorCodes.TaskActionNotFound, ex.Message, ex);
        }
        catch (TaskActionException ex)
        {
            throw new ActionException(ErrorCodes.TaskActionUnknown, ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}