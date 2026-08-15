// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerAutomate.Desktop.Modules.Tasks.Actions.Tests;

internal sealed class FakeTaskRunner : ITaskRunner
{
    public Exception? DelayException { get; set; }
    public Exception? WhenAllException { get; set; }
    public Exception? WhenAnyException { get; set; }
    public int DelayMilliseconds { get; private set; }
    public IReadOnlyList<TaskObject> LastTasks { get; private set; } = Array.Empty<TaskObject>();

    public Task Delay(int milliseconds)
    {
        DelayMilliseconds = milliseconds;
        return DelayException is null ? Task.CompletedTask : throw DelayException;
    }

    public Task WhenAll(IEnumerable<TaskObject> tasks)
    {
        LastTasks = tasks.ToList();
        return WhenAllException is null ? Task.CompletedTask : Task.FromException(WhenAllException);
    }

    public Task WhenAny(IEnumerable<TaskObject> tasks)
    {
        LastTasks = tasks.ToList();
        return WhenAnyException is null ? Task.CompletedTask : Task.FromException(WhenAnyException);
    }
}