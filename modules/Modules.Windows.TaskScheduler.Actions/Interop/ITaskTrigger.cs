// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.Win32.TaskScheduler;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

public interface ITaskTrigger : IDisposable
{
    bool Enabled { get; set; }
    DateTime EndBoundary { get; }
    TimeSpan ExecutionTimeLimit { get; }
    string Id { get; }
    ITaskRepetitionPattern Repetition { get; }
    DateTime StartBoundary { get; }
    TaskTriggerType Type { get; }
}
