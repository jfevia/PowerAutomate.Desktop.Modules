// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;

namespace PowerAutomate.Desktop.Modules.Windows.TaskScheduler.Actions.Interop;

public interface ITaskTriggerCollection : IEnumerable<ITaskTrigger>, IDisposable
{
    void Add(TaskTriggerDefinition definition);
    bool Remove(ITaskTrigger trigger);
}
