// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Abstractions;

// Thin wrapper over the system clock; it carries no branching logic of its own.
[ExcludeFromCodeCoverage]
internal sealed class SystemClock : IClock
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

    public void Sleep(int milliseconds) => Thread.Sleep(milliseconds);
}
