// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Extensions;

internal static class WindowExtensions
{
    private const int MaximumTextLength = 8192;

    /// <summary>
    /// Kept short so a hung window cannot stall discovery over many candidates.
    /// </summary>
    private const uint DiscoveryTimeoutMilliseconds = 200;

    public static string GetClassName(IntPtr handle)
    {
        var buffer = new StringBuilder(256);
        var length = NativeMethods.GetClassName(handle, buffer, buffer.Capacity);
        return length > 0 ? buffer.ToString(0, length) : string.Empty;
    }

    /// <summary>
    /// Reads window text through WM_GETTEXT, which unlike GetWindowText also works for controls owned by another process.
    /// </summary>
    public static string GetText(IntPtr handle, uint timeoutMilliseconds)
    {
        try
        {
            var length = MessageDispatcher.Send(handle, WindowMessages.GetTextLength, IntPtr.Zero, IntPtr.Zero, timeoutMilliseconds).ToInt64();
            if (length <= 0)
            {
                return string.Empty;
            }

            var capacity = (int)Math.Min(length, MaximumTextLength) + 1;
            var buffer = new StringBuilder(capacity);
            MessageDispatcher.SendBuffer(handle, WindowMessages.GetText, (IntPtr)capacity, buffer, timeoutMilliseconds);
            return buffer.ToString();
        }
        catch (Exception)
        {
            return GetCachedText(handle);
        }
    }

    /// <summary>
    /// Falls back to the window manager's cached title when the target cannot answer a message.
    /// </summary>
    private static string GetCachedText(IntPtr handle)
    {
        var length = NativeMethods.GetWindowTextLength(handle);
        if (length <= 0)
        {
            return string.Empty;
        }

        var buffer = new StringBuilder(length + 1);
        var copied = NativeMethods.GetWindowText(handle, buffer, buffer.Capacity);
        return copied > 0 ? buffer.ToString(0, copied) : string.Empty;
    }

    public static WindowObject ToWindowObject(IntPtr handle)
    {
        NativeMethods.GetWindowThreadProcessId(handle, out var processId);
        NativeMethods.GetClientRect(handle, out var clientRect);

        return new WindowObject(
            handle.ToInt64(),
            GetClassName(handle),
            GetText(handle, DiscoveryTimeoutMilliseconds),
            NativeMethods.GetDlgCtrlID(handle),
            (int)processId,
            NativeMethods.IsWindowEnabled(handle),
            NativeMethods.IsWindowVisible(handle),
            clientRect.Right - clientRect.Left,
            clientRect.Bottom - clientRect.Top);
    }

    /// <summary>
    /// Returns the client-area midpoint, used when a caller does not pin the click to a coordinate.
    /// </summary>
    public static void GetCenter(IntPtr handle, out int x, out int y)
    {
        NativeMethods.GetClientRect(handle, out var clientRect);
        x = (clientRect.Right - clientRect.Left) / 2;
        y = (clientRect.Bottom - clientRect.Top) / 2;
    }

    public static void ClientToScreen(IntPtr handle, int x, int y, out int screenX, out int screenY)
    {
        var point = new NativeMethods.Point { X = x, Y = y };
        NativeMethods.ClientToScreen(handle, ref point);
        screenX = point.X;
        screenY = point.Y;
    }

    public static bool Matches(string candidate, string? pattern, TextMatchMode matchMode)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return true;
        }

        return matchMode switch
        {
            TextMatchMode.Equals => string.Equals(candidate, pattern, StringComparison.CurrentCultureIgnoreCase),
            TextMatchMode.Contains => candidate.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase) >= 0,
            TextMatchMode.StartsWith => candidate.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase),
            _ => throw new ArgumentOutOfRangeException(nameof(matchMode), matchMode, "Unknown text match mode.")
        };
    }

    public static List<IntPtr> EnumerateTopLevelWindows()
    {
        var handles = new List<IntPtr>();
        NativeMethods.EnumWindows((handle, _) =>
        {
            handles.Add(handle);
            return true;
        }, IntPtr.Zero);
        return handles;
    }

    public static List<IntPtr> EnumerateChildWindows(IntPtr parent, bool recursive)
    {
        var handles = new List<IntPtr>();

        if (recursive)
        {
            // EnumChildWindows already walks the whole subtree.
            NativeMethods.EnumChildWindows(parent, (handle, _) =>
            {
                handles.Add(handle);
                return true;
            }, IntPtr.Zero);

            return handles;
        }

        NativeMethods.EnumChildWindows(parent, (handle, _) =>
        {
            if (NativeMethods.GetParent(handle) == parent)
            {
                handles.Add(handle);
            }

            return true;
        }, IntPtr.Zero);

        return handles;
    }
}
