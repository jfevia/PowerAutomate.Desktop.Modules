// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Abstractions;

/// <summary>
/// Time source for the discovery retry loop, so waiting can be simulated instead of really elapsing.
/// </summary>
public interface IClock
{
    long ElapsedMilliseconds { get; }

    void Sleep(int milliseconds);
}
