// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Abstractions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Services;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions;

/// <summary>
/// The collaborators every action needs, so a flow run and a test can supply different ones.
/// </summary>
public sealed class InputSimulationContext
{
    public InputSimulationContext(IWindowService windowService, IInputSender inputSender, IMessageDispatcher messageDispatcher)
    {
        WindowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
        InputSender = inputSender ?? throw new ArgumentNullException(nameof(inputSender));
        MessageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
    }

    public IInputSender InputSender { get; }
    public IMessageDispatcher MessageDispatcher { get; }
    public IWindowService WindowService { get; }

    /// <summary>
    /// Wiring used when Power Automate constructs an action through its parameterless constructor.
    /// </summary>
    public static InputSimulationContext CreateDefault()
    {
        var clock = new SystemClock();
        var nativeMethods = new NativeMethods();
        var messageDispatcher = new MessageDispatcher(nativeMethods);
        var inputSender = new InputSender(messageDispatcher, nativeMethods, clock);
        var windowService = new WindowService(nativeMethods, messageDispatcher, clock);

        return new InputSimulationContext(windowService, inputSender, messageDispatcher);
    }
}
