// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

public interface ITaskRepetitionPattern
{
    TimeSpan Duration { get; }
    TimeSpan Interval { get; }
    bool StopAtDurationEnd { get; }
}
