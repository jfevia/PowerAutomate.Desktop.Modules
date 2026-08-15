// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

/// <summary>
/// Scriptable stand-in for user32, so every Win32 outcome can be reproduced without a desktop.
/// </summary>
internal sealed class FakeNativeMethods : INativeMethods
{
    public Dictionary<IntPtr, string> ClassNames { get; } = new();
    public Dictionary<IntPtr, NativeRect> ClientRects { get; } = new();
    public List<IntPtr> ChildWindows { get; } = new();
    public Dictionary<IntPtr, int> ControlIds { get; } = new();
    public Dictionary<IntPtr, IntPtr> Parents { get; } = new();
    public Dictionary<IntPtr, uint> ProcessIds { get; } = new();
    public Dictionary<IntPtr, string> WindowTexts { get; } = new();
    public List<IntPtr> TopLevelWindows { get; } = new();

    public bool ClientToScreenResult { get; set; } = true;
    public int ClientToScreenOffsetX { get; set; }
    public int ClientToScreenOffsetY { get; set; }
    public bool EnabledResult { get; set; } = true;
    public int LastError { get; set; }
    public uint ScanCode { get; set; } = 0x1E;
    public bool PostMessageResult { get; set; } = true;
    public HashSet<IntPtr> ValidWindows { get; } = new();
    public bool VisibleResult { get; set; } = true;

    /// <summary>
    /// Result handed back by every SendMessageTimeout overload; zero means the call failed.
    /// </summary>
    public IntPtr SendOutcome { get; set; } = (IntPtr)1;

    public IntPtr SendResult { get; set; } = IntPtr.Zero;

    public List<PostedMessage> Posted { get; } = new();
    public List<SentMessage> Sent { get; } = new();

    public bool ClientToScreen(IntPtr handle, ref NativePoint point)
    {
        point.X += ClientToScreenOffsetX;
        point.Y += ClientToScreenOffsetY;
        return ClientToScreenResult;
    }

    public bool EnumChildWindows(IntPtr parent, EnumWindowsCallback callback, IntPtr parameter)
    {
        foreach (var handle in ChildWindows)
        {
            if (!callback(handle, parameter))
            {
                break;
            }
        }

        return true;
    }

    public bool EnumWindows(EnumWindowsCallback callback, IntPtr parameter)
    {
        foreach (var handle in TopLevelWindows)
        {
            if (!callback(handle, parameter))
            {
                break;
            }
        }

        return true;
    }

    public int GetClassName(IntPtr handle, StringBuilder buffer, int maximumCount)
    {
        if (!ClassNames.TryGetValue(handle, out var value))
        {
            return 0;
        }

        buffer.Append(value);
        return value.Length;
    }

    public bool GetClientRect(IntPtr handle, out NativeRect rect)
    {
        return ClientRects.TryGetValue(handle, out rect);
    }

    public int GetDlgCtrlId(IntPtr handle) => ControlIds.TryGetValue(handle, out var value) ? value : 0;

    public int GetLastError() => LastError;

    public IntPtr GetParent(IntPtr handle) => Parents.TryGetValue(handle, out var value) ? value : IntPtr.Zero;

    public int GetWindowText(IntPtr handle, StringBuilder buffer, int maximumCount)
    {
        if (FailWindowTextCopy || !WindowTexts.TryGetValue(handle, out var value))
        {
            return 0;
        }

        buffer.Append(value);
        return value.Length;
    }

    /// <summary>
    /// Reproduces a window that reports a length but then copies nothing.
    /// </summary>
    public bool FailWindowTextCopy { get; set; }

    public int GetWindowTextLength(IntPtr handle) => WindowTexts.TryGetValue(handle, out var value) ? value.Length : 0;

    public uint GetWindowThreadProcessId(IntPtr handle, out uint processId)
    {
        processId = ProcessIds.TryGetValue(handle, out var value) ? value : 0;
        return 1;
    }

    public bool IsWindow(IntPtr handle) => ValidWindows.Contains(handle);

    public bool IsWindowEnabled(IntPtr handle) => EnabledResult;

    public bool IsWindowVisible(IntPtr handle) => VisibleResult;

    public uint MapVirtualKey(uint code, uint mapType) => ScanCode;

    public bool PostMessage(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam)
    {
        Posted.Add(new PostedMessage(handle, message, wParam, lParam));
        return PostMessageResult;
    }

    public IntPtr SendMessageTimeout(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam, uint flags, uint timeout, out IntPtr result)
    {
        Sent.Add(new SentMessage(handle, message, wParam, lParam, null));
        result = SendResult;
        return SendOutcome;
    }

    public IntPtr SendMessageTimeoutBuffer(IntPtr handle, uint message, IntPtr wParam, StringBuilder lParam, uint flags, uint timeout, out IntPtr result)
    {
        Sent.Add(new SentMessage(handle, message, wParam, IntPtr.Zero, null));
        lParam.Append(BufferText);
        result = SendResult;
        return SendOutcome;
    }

    public IntPtr SendMessageTimeoutText(IntPtr handle, uint message, IntPtr wParam, string lParam, uint flags, uint timeout, out IntPtr result)
    {
        Sent.Add(new SentMessage(handle, message, wParam, IntPtr.Zero, lParam));
        result = SendResult;
        return SendOutcome;
    }

    /// <summary>
    /// Text written into the caller's buffer by the WM_GETTEXT overload.
    /// </summary>
    public string BufferText { get; set; } = string.Empty;

    public IntPtr RegisterWindow(long handle)
    {
        var pointer = (IntPtr)handle;
        ValidWindows.Add(pointer);
        return pointer;
    }

    internal readonly struct PostedMessage
    {
        public PostedMessage(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam)
        {
            Handle = handle;
            Message = message;
            WParam = wParam;
            LParam = lParam;
        }

        public IntPtr Handle { get; }
        public IntPtr LParam { get; }
        public uint Message { get; }
        public IntPtr WParam { get; }
    }

    internal readonly struct SentMessage
    {
        public SentMessage(IntPtr handle, uint message, IntPtr wParam, IntPtr lParam, string? text)
        {
            Handle = handle;
            Message = message;
            WParam = wParam;
            LParam = lParam;
            Text = text;
        }

        public IntPtr Handle { get; }
        public IntPtr LParam { get; }
        public uint Message { get; }
        public string? Text { get; }
        public IntPtr WParam { get; }
    }
}
