// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions;

public interface ITaskRunner
{
    Task Delay(int milliseconds);
    Task WhenAll(IEnumerable<TaskObject> tasks);
    Task WhenAny(IEnumerable<TaskObject> tasks);
}

// Default adapter forwards to System.Threading.Tasks.Task.
[ExcludeFromCodeCoverage]
internal sealed class TaskRunner : ITaskRunner
{
    public Task Delay(int milliseconds) => Task.Delay(milliseconds);
    public Task WhenAll(IEnumerable<TaskObject> tasks) => Task.WhenAll(tasks.Select(task => task.Task));
    public async Task WhenAny(IEnumerable<TaskObject> tasks) => await Task.WhenAny(tasks.Select(task => task.Task));
}