// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32.TaskScheduler;
using PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Tests.Fakes;

internal sealed class FakeTaskSchedulerFactory : ITaskSchedulerFactory
{
    public FakeTaskSchedulerService Service { get; } = new();
    public Exception? ConnectException { get; set; }
    public string? TargetServer { get; private set; }
    public string? UserName { get; private set; }
    public string? AccountDomain { get; private set; }
    public string? Password { get; private set; }

    public ITaskSchedulerService Connect(string targetServer, string userName, string accountDomain, string password)
    {
        TargetServer = targetServer;
        UserName = userName;
        AccountDomain = accountDomain;
        Password = password;
        if (ConnectException is not null) throw ConnectException;
        return Service;
    }
}

internal sealed class FakeTaskSchedulerService : ITaskSchedulerService
{
    public Dictionary<string, FakeTaskFolder> Folders { get; } = new(StringComparer.OrdinalIgnoreCase);
    public FakeTaskFolder Root { get; } = new();
    public Dictionary<string, FakeScheduledTask> Tasks { get; } = new(StringComparer.OrdinalIgnoreCase);
    public bool Disposed { get; private set; }
    public ITaskFolder RootFolder => Root;

    public void Dispose() => Disposed = true;

    public IScheduledTask? FindTask(string taskName) => Tasks.TryGetValue(taskName, out var task) ? task : null;

    public ITaskFolder? GetFolder(string folderPath) => Folders.TryGetValue(folderPath, out var folder) ? folder : null;

    public ITaskDefinition NewTask() => new FakeTaskDefinition();
}

internal sealed class FakeTaskFolder : ITaskFolder
{
    public List<FakeScheduledTask> Tasks { get; } = new();
    public string? DeletedTaskName { get; private set; }
    public bool DeleteThrowsFileNotFound { get; set; }
    public bool Disposed { get; private set; }
    public string? ImportedFileName { get; private set; }
    public string? ImportedTaskName { get; private set; }
    public Regex? LastFilter { get; private set; }
    public string? RegisteredTaskName { get; private set; }
    public ITaskDefinition? RegisteredDefinition { get; private set; }

    public void DeleteTask(string taskName)
    {
        if (DeleteThrowsFileNotFound) throw new FileNotFoundException();
        DeletedTaskName = taskName;
    }

    public void Dispose() => Disposed = true;

    public ITaskCollection GetTasks(Regex? filter)
    {
        LastFilter = filter;
        return new FakeTaskCollection(filter is null ? Tasks : Tasks.Where(task => filter.IsMatch(task.Name)).ToList());
    }

    public IScheduledTask ImportTask(string taskName, string fileName)
    {
        ImportedTaskName = taskName;
        ImportedFileName = fileName;
        return new FakeScheduledTask { Name = taskName };
    }

    public IScheduledTask RegisterTaskDefinition(string taskName, ITaskDefinition definition)
    {
        RegisteredTaskName = taskName;
        RegisteredDefinition = definition;
        return new FakeScheduledTask { Name = taskName };
    }
}

internal sealed class FakeTaskCollection : ITaskCollection
{
    private readonly IReadOnlyList<IScheduledTask> _tasks;

    public FakeTaskCollection(IReadOnlyList<FakeScheduledTask> tasks) => _tasks = tasks;

    public bool Disposed { get; private set; }

    public void Dispose() => Disposed = true;

    public IEnumerator<IScheduledTask> GetEnumerator() => _tasks.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class FakeScheduledTask : IScheduledTask
{
    public ITaskDefinition Definition { get; } = new FakeTaskDefinition();
    public bool Disposed { get; private set; }
    public bool Enabled { get; set; }
    public ITaskFolder Folder { get; set; } = new FakeTaskFolder();
    public bool IsReadOnly { get; set; }
    public DateTime LastRunTime { get; set; } = new(2024, 1, 2);
    public int LastTaskResult { get; set; } = 5;
    public string Name { get; set; } = string.Empty;
    public DateTime NextRunTime { get; set; } = new(2024, 1, 3);
    public int NumberOfMissedRuns { get; set; } = 7;
    public string Path { get; set; } = string.Empty;
    public TaskState State { get; set; }
    public string? ExportedFileName { get; private set; }
    public bool Ran { get; private set; }
    public bool Stopped { get; private set; }

    public void Dispose() => Disposed = true;

    public void Export(string fileName) => ExportedFileName = fileName;

    public void Run() => Ran = true;

    public void Stop() => Stopped = true;
}

internal sealed class FakeTaskDefinition : ITaskDefinition
{
    public FakeTaskActionCollection ActionCollection { get; } = new();
    public FakeTaskTriggerCollection TriggerCollection { get; } = new();
    public bool Disposed { get; private set; }
    public ITaskActionCollection Actions => ActionCollection;
    public ITaskTriggerCollection Triggers => TriggerCollection;

    public void Dispose() => Disposed = true;
}

internal sealed class FakeTaskActionCollection : ITaskActionCollection
{
    public List<FakeTaskAction> Actions { get; } = new();
    public (string Path, string Arguments, string WorkingDirectory)? AddedExec { get; private set; }
    public bool Disposed { get; private set; }
    public bool RemoveResult { get; set; } = true;
    public bool RemoveThrowsFileNotFound { get; set; }

    public void AddExec(string path, string arguments, string workingDirectory) => AddedExec = (path, arguments, workingDirectory);

    public void Dispose() => Disposed = true;

    public IEnumerator<ITaskAction> GetEnumerator() => Actions.Cast<ITaskAction>().GetEnumerator();

    public bool Remove(ITaskAction action)
    {
        if (RemoveThrowsFileNotFound) throw new FileNotFoundException();
        return RemoveResult && Actions.Remove((FakeTaskAction)action);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class FakeTaskAction : ITaskAction
{
    public string Id { get; set; } = string.Empty;
    public TaskActionType Type { get; set; }
    public bool Disposed { get; private set; }

    public void Dispose() => Disposed = true;
}

internal sealed class FakeTaskTriggerCollection : ITaskTriggerCollection
{
    public List<FakeTaskTrigger> Triggers { get; } = new();
    public TaskTriggerDefinition? Added { get; private set; }
    public bool Disposed { get; private set; }
    public bool RemoveResult { get; set; } = true;
    public bool RemoveThrowsFileNotFound { get; set; }

    public void Add(TaskTriggerDefinition definition) => Added = definition;

    public void Dispose() => Disposed = true;

    public IEnumerator<ITaskTrigger> GetEnumerator() => Triggers.Cast<ITaskTrigger>().GetEnumerator();

    public bool Remove(ITaskTrigger trigger)
    {
        if (RemoveThrowsFileNotFound) throw new FileNotFoundException();
        return RemoveResult && Triggers.Remove((FakeTaskTrigger)trigger);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class FakeTaskTrigger : ITaskTrigger
{
    public bool Enabled { get; set; }
    public DateTime EndBoundary { get; set; } = new(2024, 3, 4);
    public TimeSpan ExecutionTimeLimit { get; set; } = TimeSpan.FromMinutes(9);
    public string Id { get; set; } = string.Empty;
    public ITaskRepetitionPattern Repetition { get; set; } = new FakeTaskRepetitionPattern();
    public DateTime StartBoundary { get; set; } = new(2024, 3, 1);
    public TaskTriggerType Type { get; set; }
    public bool Disposed { get; private set; }

    public void Dispose() => Disposed = true;
}

internal sealed class FakeTaskRepetitionPattern : ITaskRepetitionPattern
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(5);
    public bool StopAtDurationEnd { get; set; } = true;
}
