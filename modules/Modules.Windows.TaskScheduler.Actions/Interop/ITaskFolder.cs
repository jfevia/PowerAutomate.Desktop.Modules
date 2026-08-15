// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Text.RegularExpressions;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

public interface ITaskFolder : IDisposable
{
    ITaskCollection GetTasks(Regex? filter);
    IScheduledTask ImportTask(string taskName, string fileName);
    IScheduledTask RegisterTaskDefinition(string taskName, ITaskDefinition definition);
    void DeleteTask(string taskName);
}
