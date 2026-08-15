// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.Win32.TaskScheduler;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

public interface IScheduledTask : IDisposable
{
    ITaskDefinition Definition { get; }
    bool Enabled { get; set; }
    ITaskFolder Folder { get; }
    bool IsReadOnly { get; }
    DateTime LastRunTime { get; }
    int LastTaskResult { get; }
    string Name { get; }
    DateTime NextRunTime { get; }
    int NumberOfMissedRuns { get; }
    string Path { get; }
    TaskState State { get; }
    void Export(string fileName);
    void Run();
    void Stop();
}
