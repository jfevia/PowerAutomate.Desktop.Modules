// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Actions.Shared;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions;

[Action(Id = "GetTasksInFolder")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetTasksInFolderAction : ActionBase
{
    [InputArgument(Order = 2, Required = false)]
    public string Filter { get; set; } = null!;

    [InputArgument(Order = 1)]
    public string FolderName { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public List<string> TaskNames { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            using var taskService = new TaskService();
            Regex? regex = null;

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                regex = new Regex(Filter);
            }

            using var taskCollection = taskService.GetFolder(FolderName)
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