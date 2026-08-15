// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Text;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

/// <summary>
/// Delivers window messages to a target window queue, bypassing the system input stack.
/// </summary>
public interface IMessageDispatcher
{
    void Post(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam);
    IntPtr Send(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam);
    IntPtr SendBuffer(IntPtr handle, uint message, IntPtr wParam, StringBuilder lParam);
    IntPtr SendText(IntPtr handle, uint message, IntPtr wParam, string lParam);
}
