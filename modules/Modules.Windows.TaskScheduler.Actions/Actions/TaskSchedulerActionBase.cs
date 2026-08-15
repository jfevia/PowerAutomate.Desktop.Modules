// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Exceptions;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Actions;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public abstract class TaskSchedulerActionBase : ActionBase
{
    protected TaskSchedulerActionBase() : this(TaskSchedulerContext.CreateDefault())
    {
    }

    protected TaskSchedulerActionBase(TaskSchedulerContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    protected TaskSchedulerContext Context { get; }

    public override void Execute(ActionContext context)
    {
        try
        {
            Run(context);
        }
        catch (Exception ex)
        {
            throw ex.ToActionException();
        }
    }

    protected abstract void Run(ActionContext context);

    protected ITaskSchedulerService Connect(string targetServer, string userName, string accountDomain, string password) =>
        Context.SchedulerFactory.Connect(targetServer, userName, accountDomain, password);

    protected static IScheduledTask RequireTask(ITaskSchedulerService service, string taskName) =>
        service.FindTask(taskName) ?? throw new TaskNotFoundException(taskName);

    protected static ITaskFolder RequireFolder(ITaskSchedulerService service, string folderPath) =>
        service.GetFolder(folderPath) ?? throw new FolderNotFoundException(folderPath);

    protected static ITaskAction RequireAction(IScheduledTask task, string actionId) =>
        task.Definition.Actions.FirstOrDefault(action => action.Id == actionId) ?? throw new TaskActionNotFoundException(task.Name, actionId);

    protected static ITaskTrigger RequireTrigger(IScheduledTask task, string triggerId) =>
        task.Definition.Triggers.FirstOrDefault(trigger => trigger.Id == triggerId) ?? throw new TaskTriggerNotFoundException(task.Name, triggerId);


    protected static void DeleteTask(IScheduledTask task)
    {
        try
        {
            task.Folder.DeleteTask(task.Name);
        }
        catch (FileNotFoundException)
        {
            throw new TaskNotFoundException(task.Name);
        }
    }
    protected static void RemoveAction(IScheduledTask task, string actionId)
    {
        try
        {
            using var action = RequireAction(task, actionId);
            if (!task.Definition.Actions.Remove(action))
            {
                throw new TaskActionException(task.Name, actionId, $"Could not delete action '{actionId}' in task '{task.Name}'");
            }
        }
        catch (FileNotFoundException)
        {
            throw new TaskActionNotFoundException(task.Name, actionId);
        }
    }

    protected static void RemoveTrigger(IScheduledTask task, string triggerId)
    {
        try
        {
            using var trigger = RequireTrigger(task, triggerId);
            if (!task.Definition.Triggers.Remove(trigger))
            {
                throw new TaskTriggerException(task.Name, triggerId, $"Could not delete trigger '{triggerId}' in task '{task.Name}'");
            }
        }
        catch (FileNotFoundException)
        {
            throw new TaskTriggerNotFoundException(task.Name, triggerId);
        }
    }
}
