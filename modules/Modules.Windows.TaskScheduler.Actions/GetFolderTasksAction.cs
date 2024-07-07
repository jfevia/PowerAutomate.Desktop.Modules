// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Actions.Shared;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

[Action(Id = "GetFolderTasks")]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetFolderTasksAction : ActionBase
{
    [InputArgument(Order = 5, Required = false)]
    public string AccountDomain { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.General, Required = false)]
    public string Filter { get; set; } = null!;

    [InputArgument(Order = 1, Group = Groups.General)]
    [DefaultValue("\\")]
    public string FolderPath { get; set; } = null!;

    [InputArgument(Order = 6, Required = false)]
    public string Password { get; set; } = null!;

    [InputArgument(Order = 3, Required = false)]
    public string TargetServer { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public List<string> TaskNames { get; set; } = null!;

    [InputArgument(Order = 4, Required = false)]
    public string UserName { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            using var taskService = new TaskService(TargetServer, UserName, AccountDomain, Password);
            Regex? regex = null;

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                regex = new Regex(Filter);
            }

            using var taskCollection = taskService.GetFolder(FolderPath)
                                                  .GetTasks(regex);
            var taskNames = new List<string>();

            foreach (var task in taskCollection)
            {
                taskNames.Add(task.Name);
                task.Dispose();
            }

            TaskNames = taskNames;
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}