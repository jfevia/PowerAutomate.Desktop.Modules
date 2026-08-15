// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

/// <summary>
/// Assembles a context whose collaborators are all fakes, and keeps them reachable for assertions.
/// </summary>
internal sealed class ActionHarness
{
    public ActionHarness()
    {
        WindowService = new FakeWindowService();
        InputSender = new FakeInputSender();
        MessageDispatcher = new FakeMessageDispatcher();
        Context = new InputSimulationContext(WindowService, InputSender, MessageDispatcher);
    }

    public InputSimulationContext Context { get; }
    public FakeInputSender InputSender { get; }
    public FakeMessageDispatcher MessageDispatcher { get; }
    public FakeWindowService WindowService { get; }

    public static WindowObject Window(long handle = 100) => new(handle, "Class", "Title", 7, 42, true, true, 200, 100);

    public static IntPtr Handle(long handle = 100) => (IntPtr)handle;
}
