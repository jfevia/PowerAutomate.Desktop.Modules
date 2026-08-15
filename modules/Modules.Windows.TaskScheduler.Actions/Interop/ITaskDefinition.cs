// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

public interface ITaskDefinition : IDisposable
{
    ITaskActionCollection Actions { get; }
    ITaskTriggerCollection Triggers { get; }
}
