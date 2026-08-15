// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Text;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

/// <summary>
/// The Win32 surface this module depends on, isolated so behaviour can be exercised without a real desktop.
/// </summary>
public interface INativeMethods
{
    bool ClientToScreen(IntPtr handle, ref NativePoint point);
    bool EnumChildWindows(IntPtr parent, EnumWindowsCallback callback, IntPtr parameter);
    bool EnumWindows(EnumWindowsCallback callback, IntPtr parameter);
    int GetClassName(IntPtr handle, StringBuilder buffer, int maximumCount);
    bool GetClientRect(IntPtr handle, out NativeRect rect);
    int GetDlgCtrlId(IntPtr handle);

    /// <summary>
    /// Last error recorded by the preceding interop call.
    /// </summary>
    int GetLastError();

    IntPtr GetParent(IntPtr handle);
    int GetWindowText(IntPtr handle, StringBuilder buffer, int maximumCount);
    int GetWindowTextLength(IntPtr handle);
    uint GetWindowThreadProcessId(IntPtr handle, out uint processId);
    bool IsWindow(IntPtr handle);
    bool IsWindowEnabled(IntPtr handle);
    bool IsWindowVisible(IntPtr handle);
    uint MapVirtualKey(uint code, uint mapType);
    bool PostMessage(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam);
    IntPtr SendMessageTimeout(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam, uint flags, uint timeout, out IntPtr result);
    IntPtr SendMessageTimeoutBuffer(IntPtr handle, uint message, IntPtr wParam, StringBuilder lParam, uint flags, uint timeout, out IntPtr result);
    IntPtr SendMessageTimeoutText(IntPtr handle, uint message, IntPtr wParam, string lParam, uint flags, uint timeout, out IntPtr result);
}

public delegate bool EnumWindowsCallback(IntPtr handle, IntPtr parameter);
