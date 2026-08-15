// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

public interface ITaskSchedulerFactory
{
    ITaskSchedulerService Connect(string targetServer, string userName, string accountDomain, string password);
}
