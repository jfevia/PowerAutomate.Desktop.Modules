// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

public interface ITaskActionCollection : IEnumerable<ITaskAction>, IDisposable
{
    void AddExec(string path, string arguments, string workingDirectory);
    bool Remove(ITaskAction action);
}
