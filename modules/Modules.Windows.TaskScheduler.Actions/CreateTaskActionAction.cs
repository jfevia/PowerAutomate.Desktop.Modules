// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Actions.Shared;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

[Action(Id = "CreateTaskAction")]
[Group(Name = "General", Order = 1)]
[Group(Name = "Advanced", Order = 2, IsDefault = true)]
[Throws(ErrorCodes.TaskNotFound)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class CreateTaskActionAction : ActionBase
{
    [InputArgument(Order = 7, Required = false)]
    public string AccountDomain { get; set; } = null!;

    [InputArgument(Order = 3, Group = "General")]
    public string Arguments { get; set; } = null!;

    [InputArgument(Order = 8, Required = false)]
    public string Password { get; set; } = null!;

    [InputArgument(Order = 2, Group = "General")]
    public string Path { get; set; } = null!;

    [InputArgument(Order = 5, Required = false)]
    public string TargetServer { get; set; } = null!;

    [InputArgument(Order = 1, Group = "General")]
    public string TaskName { get; set; } = null!;

    [InputArgument(Order = 6, Required = false)]
    public string UserName { get; set; } = null!;

    [InputArgument(Order = 4, Group = "General")]
    public string WorkingDirectory { get; set; } = null!;

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

            using var execAction = new ExecAction(Path, Arguments, WorkingDirectory);
            task.Definition.Actions.Add(execAction);
        }
        catch (TaskNotFoundException ex)
        {
            throw new ActionException(ErrorCodes.TaskNotFound, ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}