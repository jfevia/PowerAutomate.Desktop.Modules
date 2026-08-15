// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

/// <summary>
/// Builds the message sequences Windows itself would produce for mouse and keyboard input.
/// </summary>
public interface IInputSender
{
    void Click(IntPtr handle, MouseButton button, int x, int y, bool isDoubleClick);
    void Scroll(IntPtr handle, ScrollDirection direction, int notches, int screenX, int screenY);
    void SendKey(IntPtr handle, VirtualKey key);
    void SendText(IntPtr handle, string text, int delayMilliseconds);
}
