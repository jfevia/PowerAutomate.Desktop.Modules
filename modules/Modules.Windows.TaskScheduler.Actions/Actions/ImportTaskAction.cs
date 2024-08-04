// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.Win32.TaskScheduler;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Actions;

[Action(Id = "ImportTask")]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class ImportTaskAction : ActionBase
{
    [InputArgument(Order = 5, Required = false)]
    public string AccountDomain { get; set; } = null!;

    [InputArgument(Order = 2)]
    public string FileName { get; set; } = null!;

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
        try
        {
            using var taskService = new TaskService(TargetServer, UserName, AccountDomain, Password);
            taskService.RootFolder.ImportTask(TaskName, FileName).Dispose();
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}