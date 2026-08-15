// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Extensions;
using NativeAction = Microsoft.Win32.TaskScheduler.Action;
using NativeTask = Microsoft.Win32.TaskScheduler.Task;
using NativeTaskState = Microsoft.Win32.TaskScheduler.TaskState;
using NativeTriggerType = Microsoft.Win32.TaskScheduler.TaskTriggerType;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

// Pure adapter around Microsoft.Win32.TaskScheduler.
[ExcludeFromCodeCoverage]
internal sealed class NativeTaskSchedulerFactory : ITaskSchedulerFactory
{
    public ITaskSchedulerService Connect(string targetServer, string userName, string accountDomain, string password) =>
        new NativeTaskSchedulerService(new TaskService(targetServer, userName, accountDomain, password));

    private sealed class NativeTaskSchedulerService : ITaskSchedulerService
    {
        private readonly TaskService _service;

        public NativeTaskSchedulerService(TaskService service) => _service = service;

        public ITaskFolder RootFolder => new NativeTaskFolder(_service.RootFolder);

        public void Dispose() => _service.Dispose();

        public IScheduledTask? FindTask(string taskName)
        {
            var task = _service.FindTask(taskName);
            return task is null ? null : new NativeScheduledTask(task);
        }

        public ITaskFolder? GetFolder(string folderPath)
        {
            var folder = _service.GetFolder(folderPath);
            return folder is null ? null : new NativeTaskFolder(folder);
        }

        public ITaskDefinition NewTask() => new NativeTaskDefinition(_service.NewTask());
    }

    private sealed class NativeTaskFolder : ITaskFolder
    {
        private readonly TaskFolder _folder;

        public NativeTaskFolder(TaskFolder folder) => _folder = folder;

        public void DeleteTask(string taskName) => _folder.DeleteTask(taskName);

        public void Dispose() => _folder.Dispose();

        public ITaskCollection GetTasks(Regex? filter) => new NativeTaskCollection(_folder.GetTasks(filter));

        public IScheduledTask ImportTask(string taskName, string fileName) => new NativeScheduledTask(_folder.ImportTask(taskName, fileName));

        public IScheduledTask RegisterTaskDefinition(string taskName, ITaskDefinition definition) =>
            new NativeScheduledTask(_folder.RegisterTaskDefinition(taskName, ((NativeTaskDefinition)definition).Definition));
    }

    private sealed class NativeTaskCollection : ITaskCollection
    {
        private readonly TaskCollection _tasks;

        public NativeTaskCollection(TaskCollection tasks) => _tasks = tasks;

        public void Dispose() => _tasks.Dispose();

        public IEnumerator<IScheduledTask> GetEnumerator() => _tasks.Cast<NativeTask>().Select(task => new NativeScheduledTask(task)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed class NativeScheduledTask : IScheduledTask
    {
        private readonly NativeTask _task;

        public NativeScheduledTask(NativeTask task) => _task = task;

        public ITaskDefinition Definition => new NativeTaskDefinition(_task.Definition);
        public bool Enabled { get => _task.Enabled; set => _task.Enabled = value; }
        public ITaskFolder Folder => new NativeTaskFolder(_task.Folder);
        public bool IsReadOnly => _task.ReadOnly;
        public DateTime LastRunTime => _task.LastRunTime;
        public int LastTaskResult => _task.LastTaskResult;
        public string Name => _task.Name;
        public DateTime NextRunTime => _task.NextRunTime;
        public int NumberOfMissedRuns => _task.NumberOfMissedRuns;
        public string Path => _task.Path;
        public NativeTaskState State => _task.State;

        public void Dispose() => _task.Dispose();

        public void Export(string fileName) => _task.Export(fileName);

        public void Run() => _task.Run();

        public void Stop() => _task.Stop();
    }

    private sealed class NativeTaskDefinition : ITaskDefinition
    {
        public NativeTaskDefinition(TaskDefinition definition) => Definition = definition;

        public TaskDefinition Definition { get; }
        public ITaskActionCollection Actions => new NativeTaskActionCollection(Definition.Actions);
        public ITaskTriggerCollection Triggers => new NativeTaskTriggerCollection(Definition.Triggers);

        public void Dispose() => Definition.Dispose();
    }

    private sealed class NativeTaskActionCollection : ITaskActionCollection
    {
        private readonly ActionCollection _actions;

        public NativeTaskActionCollection(ActionCollection actions) => _actions = actions;

        public void AddExec(string path, string arguments, string workingDirectory) => _actions.Add(new ExecAction(path, arguments, workingDirectory));

        public void Dispose() => _actions.Dispose();

        public IEnumerator<ITaskAction> GetEnumerator() => _actions.Cast<NativeAction>().Select(action => new NativeTaskAction(action)).GetEnumerator();

        public bool Remove(ITaskAction action) => _actions.Remove(((NativeTaskAction)action).Action);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed class NativeTaskAction : ITaskAction
    {
        public NativeTaskAction(NativeAction action) => Action = action;

        public NativeAction Action { get; }
        public string Id => Action.Id;
        public TaskActionType Type => Action.ActionType;

        public void Dispose() => Action.Dispose();
    }

    private sealed class NativeTaskTriggerCollection : ITaskTriggerCollection
    {
        private readonly TriggerCollection _triggers;

        public NativeTaskTriggerCollection(TriggerCollection triggers) => _triggers = triggers;

        public void Add(TaskTriggerDefinition definition) => _triggers.Add(CreateTrigger(definition));

        public void Dispose() => _triggers.Dispose();

        public IEnumerator<ITaskTrigger> GetEnumerator() => _triggers.Cast<Trigger>().Select(trigger => new NativeTaskTrigger(trigger)).GetEnumerator();

        public bool Remove(ITaskTrigger trigger) => _triggers.Remove(((NativeTaskTrigger)trigger).Trigger);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static Trigger CreateTrigger(TaskTriggerDefinition definition)
        {
            var trigger = (Trigger)(definition.Type switch
            {
                Enums.TriggerType.Boot => new BootTrigger { Delay = definition.Delay },
                Enums.TriggerType.Daily => new DailyTrigger { StartBoundary = definition.StartBoundary, DaysInterval = definition.DaysInterval, RandomDelay = definition.RandomDelay },
                Enums.TriggerType.Event => new EventTrigger { Subscription = definition.Subscription, ValueQueries = { ["Name"] = "Value" } },
                Enums.TriggerType.Idle => new IdleTrigger { StartBoundary = definition.StartBoundary },
                Enums.TriggerType.Logon => new LogonTrigger { Delay = definition.Delay, UserId = definition.UserId },
                Enums.TriggerType.MonthlyDayOfWeek => new MonthlyDOWTrigger { StartBoundary = definition.StartBoundary, DaysOfWeek = definition.DaysOfWeek.ToAbstraction(), MonthsOfYear = definition.MonthsOfYear.ToAbstraction(), WeeksOfMonth = definition.WeeksOfMonth.ToAbstraction() },
                Enums.TriggerType.Monthly => new MonthlyTrigger { StartBoundary = definition.StartBoundary, DaysOfMonth = definition.DaysOfMonth.ToArray(), MonthsOfYear = definition.MonthsOfYear.ToAbstraction(), RunOnLastDayOfMonth = definition.RunOnLastDayOfMonth },
                Enums.TriggerType.Registration => new RegistrationTrigger { Delay = definition.Delay },
                Enums.TriggerType.SessionStateChange => new SessionStateChangeTrigger { StateChange = definition.State.ToAbstraction() },
                Enums.TriggerType.Time => new TimeTrigger { StartBoundary = definition.StartBoundary },
                Enums.TriggerType.Weekly => new WeeklyTrigger { StartBoundary = definition.StartBoundary, DaysOfWeek = definition.DaysOfWeek.ToAbstraction(), WeeksInterval = definition.WeeksInterval },
                _ => throw new ArgumentOutOfRangeException()
            });
            trigger.Id = definition.Id;
            trigger.Enabled = definition.Enabled;
            trigger.StartBoundary = definition.StartBoundary;
            trigger.EndBoundary = definition.EndBoundary;
            trigger.ExecutionTimeLimit = definition.Timeout;
            trigger.Repetition.Duration = definition.RepetitionDuration;
            trigger.Repetition.Interval = definition.RepetitionInterval;
            trigger.Repetition.StopAtDurationEnd = definition.RepetitionStopAtDurationEnd;
            return trigger;
        }
    }

    private sealed class NativeTaskTrigger : ITaskTrigger
    {
        public NativeTaskTrigger(Trigger trigger) => Trigger = trigger;

        public bool Enabled { get => Trigger.Enabled; set => Trigger.Enabled = value; }
        public DateTime EndBoundary => Trigger.EndBoundary;
        public TimeSpan ExecutionTimeLimit => Trigger.ExecutionTimeLimit;
        public string Id => Trigger.Id;
        public ITaskRepetitionPattern Repetition => new NativeTaskRepetitionPattern(Trigger.Repetition);
        public DateTime StartBoundary => Trigger.StartBoundary;
        public Trigger Trigger { get; }
        public NativeTriggerType Type => Trigger.TriggerType;

        public void Dispose() => Trigger.Dispose();
    }

    private sealed class NativeTaskRepetitionPattern : ITaskRepetitionPattern
    {
        private readonly RepetitionPattern _repetition;

        public NativeTaskRepetitionPattern(RepetitionPattern repetition) => _repetition = repetition;

        public TimeSpan Duration => _repetition.Duration;
        public TimeSpan Interval => _repetition.Interval;
        public bool StopAtDurationEnd => _repetition.StopAtDurationEnd;
    }
}
