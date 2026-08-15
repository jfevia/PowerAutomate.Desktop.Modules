// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

public interface ITaskSchedulerService : IDisposable
{
    ITaskFolder RootFolder { get; }
    IScheduledTask? FindTask(string taskName);
    ITaskFolder? GetFolder(string folderPath);
    ITaskDefinition NewTask();
}
