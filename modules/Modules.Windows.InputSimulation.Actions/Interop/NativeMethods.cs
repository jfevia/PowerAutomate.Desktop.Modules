// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

// Pure interop adapter: every member forwards straight to user32 and holds no logic to exercise.
[ExcludeFromCodeCoverage]
internal sealed class NativeMethods : INativeMethods
{
    public bool ClientToScreen(IntPtr handle, ref NativePoint point) => NativeClientToScreen(handle, ref point);

    public bool EnumChildWindows(IntPtr parent, EnumWindowsCallback callback, IntPtr parameter) => NativeEnumChildWindows(parent, callback, parameter);

    public bool EnumWindows(EnumWindowsCallback callback, IntPtr parameter) => NativeEnumWindows(callback, parameter);

    public int GetClassName(IntPtr handle, StringBuilder buffer, int maximumCount) => NativeGetClassName(handle, buffer, maximumCount);

    public bool GetClientRect(IntPtr handle, out NativeRect rect) => NativeGetClientRect(handle, out rect);

    public int GetDlgCtrlId(IntPtr handle) => NativeGetDlgCtrlID(handle);

    public int GetLastError() => Marshal.GetLastWin32Error();

    public IntPtr GetParent(IntPtr handle) => NativeGetParent(handle);

    public int GetWindowText(IntPtr handle, StringBuilder buffer, int maximumCount) => NativeGetWindowText(handle, buffer, maximumCount);

    public int GetWindowTextLength(IntPtr handle) => NativeGetWindowTextLength(handle);

    public uint GetWindowThreadProcessId(IntPtr handle, out uint processId) => NativeGetWindowThreadProcessId(handle, out processId);

    public bool IsWindow(IntPtr handle) => NativeIsWindow(handle);

    public bool IsWindowEnabled(IntPtr handle) => NativeIsWindowEnabled(handle);

    public bool IsWindowVisible(IntPtr handle) => NativeIsWindowVisible(handle);

    public uint MapVirtualKey(uint code, uint mapType) => NativeMapVirtualKey(code, mapType);

    public bool PostMessage(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam) => NativePostMessage(handle, message, wParam, lParam);

    public IntPtr SendMessageTimeout(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam, uint flags, uint timeout, out IntPtr result) =>
        NativeSendMessageTimeout(handle, message, wParam, lParam, flags, timeout, out result);

    public IntPtr SendMessageTimeoutBuffer(IntPtr handle, uint message, IntPtr wParam, StringBuilder lParam, uint flags, uint timeout, out IntPtr result) =>
        NativeSendMessageTimeoutBuffer(handle, message, wParam, lParam, flags, timeout, out result);

    public IntPtr SendMessageTimeoutText(IntPtr handle, uint message, IntPtr wParam, string lParam, uint flags, uint timeout, out IntPtr result) =>
        NativeSendMessageTimeoutText(handle, message, wParam, lParam, flags, timeout, out result);

    [DllImport("user32.dll", EntryPoint = "ClientToScreen", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeClientToScreen(IntPtr hWnd, ref NativePoint lpPoint);

    [DllImport("user32.dll", EntryPoint = "EnumChildWindows", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeEnumChildWindows(IntPtr hWndParent, EnumWindowsCallback lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeEnumWindows(EnumWindowsCallback lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClassNameW", SetLastError = true)]
    private static extern int NativeGetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", EntryPoint = "GetClientRect", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeGetClientRect(IntPtr hWnd, out NativeRect lpRect);

    [DllImport("user32.dll", EntryPoint = "GetDlgCtrlID", SetLastError = true)]
    private static extern int NativeGetDlgCtrlID(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
    private static extern IntPtr NativeGetParent(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetWindowTextW", SetLastError = true)]
    private static extern int NativeGetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetWindowTextLengthW", SetLastError = true)]
    private static extern int NativeGetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true)]
    private static extern uint NativeGetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll", EntryPoint = "IsWindow", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeIsWindow(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "IsWindowEnabled", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeIsWindowEnabled(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "IsWindowVisible", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativeIsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", EntryPoint = "MapVirtualKeyW", SetLastError = true)]
    private static extern uint NativeMapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "PostMessageW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool NativePostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
    private static extern IntPtr NativeSendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
    private static extern IntPtr NativeSendMessageTimeoutBuffer(IntPtr hWnd, uint msg, IntPtr wParam, StringBuilder lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageTimeoutW", SetLastError = true)]
    private static extern IntPtr NativeSendMessageTimeoutText(IntPtr hWnd, uint msg, IntPtr wParam, string lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);
}
