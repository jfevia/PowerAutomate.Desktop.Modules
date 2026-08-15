// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Abstractions;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

/// <summary>
/// Virtual clock: sleeping advances time instantly so retry loops finish without real waiting.
/// </summary>
internal sealed class FakeClock : IClock
{
    public long ElapsedMilliseconds { get; set; }

    public List<int> Sleeps { get; } = new();

    public void Sleep(int milliseconds)
    {
        Sleeps.Add(milliseconds);
        ElapsedMilliseconds += milliseconds;
    }
}
