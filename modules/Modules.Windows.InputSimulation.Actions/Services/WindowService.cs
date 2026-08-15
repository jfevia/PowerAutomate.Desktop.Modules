// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Abstractions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Services;

/// <inheritdoc />
public sealed class WindowService : IWindowService
{
    private const int MaximumTextLength = 8192;
    private const int PollIntervalMilliseconds = 100;

    private readonly IClock _clock;
    private readonly IMessageDispatcher _messageDispatcher;
    private readonly INativeMethods _nativeMethods;

    public WindowService(INativeMethods nativeMethods, IMessageDispatcher messageDispatcher, IClock clock)
    {
        _nativeMethods = nativeMethods ?? throw new ArgumentNullException(nameof(nativeMethods));
        _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public string GetClassName(IntPtr handle)
    {
        var buffer = new StringBuilder(256);
        var length = _nativeMethods.GetClassName(handle, buffer, buffer.Capacity);
        return length > 0 ? buffer.ToString() : string.Empty;
    }

    /// <summary>
    /// Reads window text through WM_GETTEXT, which unlike GetWindowText also works for controls owned by another process.
    /// </summary>
    public string GetText(IntPtr handle)
    {
        try
        {
            var length = _messageDispatcher.Send(handle, WindowMessages.GetTextLength, IntPtr.Zero, IntPtr.Zero).ToInt64();
            if (length <= 0)
            {
                return string.Empty;
            }

            var capacity = (int)Math.Min(length, MaximumTextLength) + 1;
            var buffer = new StringBuilder(capacity);
            _messageDispatcher.SendBuffer(handle, WindowMessages.GetText, (IntPtr)capacity, buffer);
            return buffer.ToString();
        }
        catch (Exception)
        {
            return GetCachedText(handle);
        }
    }

    public int GetControlId(IntPtr handle) => _nativeMethods.GetDlgCtrlId(handle);

    public IntPtr GetParent(IntPtr handle) => _nativeMethods.GetParent(handle);

    public WindowObject ToWindowObject(IntPtr handle)
    {
        _nativeMethods.GetWindowThreadProcessId(handle, out var processId);
        _nativeMethods.GetClientRect(handle, out var clientRect);

        return new WindowObject(
            handle.ToInt64(),
            GetClassName(handle),
            GetText(handle),
            _nativeMethods.GetDlgCtrlId(handle),
            (int)processId,
            _nativeMethods.IsWindowEnabled(handle),
            _nativeMethods.IsWindowVisible(handle),
            clientRect.Right - clientRect.Left,
            clientRect.Bottom - clientRect.Top);
    }

    /// <summary>
    /// Returns the client-area midpoint, used when a caller does not pin the click to a coordinate.
    /// </summary>
    public void GetCenter(IntPtr handle, out int x, out int y)
    {
        _nativeMethods.GetClientRect(handle, out var clientRect);
        x = (clientRect.Right - clientRect.Left) / 2;
        y = (clientRect.Bottom - clientRect.Top) / 2;
    }

    public void ClientToScreen(IntPtr handle, int x, int y, out int screenX, out int screenY)
    {
        var point = new NativePoint { X = x, Y = y };
        _nativeMethods.ClientToScreen(handle, ref point);
        screenX = point.X;
        screenY = point.Y;
    }

    public bool Matches(string candidate, string pattern, TextMatchMode matchMode)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return true;
        }

        var text = candidate ?? string.Empty;

        return matchMode switch
        {
            TextMatchMode.Equals => string.Equals(text, pattern, StringComparison.CurrentCultureIgnoreCase),
            TextMatchMode.Contains => text.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase) >= 0,
            TextMatchMode.StartsWith => text.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase),
            _ => throw new ArgumentOutOfRangeException(nameof(matchMode), matchMode, "Unknown text match mode.")
        };
    }

    public IReadOnlyList<IntPtr> EnumerateTopLevelWindows()
    {
        var handles = new List<IntPtr>();
        _nativeMethods.EnumWindows((handle, _) =>
        {
            handles.Add(handle);
            return true;
        }, IntPtr.Zero);
        return handles;
    }

    public IReadOnlyList<IntPtr> EnumerateChildWindows(IntPtr parent, bool recursive)
    {
        var handles = new List<IntPtr>();
        _nativeMethods.EnumChildWindows(parent, (handle, _) =>
        {
            // EnumChildWindows walks the whole subtree, so direct children are filtered out afterwards.
            if (recursive || _nativeMethods.GetParent(handle) == parent)
            {
                handles.Add(handle);
            }

            return true;
        }, IntPtr.Zero);
        return handles;
    }

    public IntPtr FindWindow(string title, string className, int? processId, TextMatchMode matchMode, int timeoutMilliseconds)
    {
        return FindFirst(
            EnumerateTopLevelWindows,
            candidate => MatchesWindow(candidate, title, className, processId, matchMode),
            timeoutMilliseconds);
    }

    public IntPtr FindControl(IntPtr parent, string text, string className, int? controlId, TextMatchMode matchMode, bool recursive, int timeoutMilliseconds)
    {
        return FindFirst(
            () => EnumerateChildWindows(parent, recursive),
            candidate => MatchesControl(candidate, text, className, controlId, matchMode),
            timeoutMilliseconds);
    }

    /// <summary>
    /// Builds a human readable description of the search terms for error messages.
    /// </summary>
    public string DescribeCriteria(params SearchCriterion[] criteria)
    {
        var parts = new List<string>();

        foreach (var criterion in criteria)
        {
            if (criterion.Value is null || criterion.Value as string == string.Empty)
            {
                continue;
            }

            parts.Add($"{criterion.Name} '{criterion.Value}'");
        }

        return parts.Count == 0 ? "the supplied criteria" : string.Join(", ", parts);
    }

    /// <summary>
    /// Falls back to the window manager's cached title when the target cannot answer a message.
    /// </summary>
    private string GetCachedText(IntPtr handle)
    {
        var length = _nativeMethods.GetWindowTextLength(handle);
        if (length <= 0)
        {
            return string.Empty;
        }

        var buffer = new StringBuilder(length + 1);
        var copied = _nativeMethods.GetWindowText(handle, buffer, buffer.Capacity);
        return copied > 0 ? buffer.ToString() : string.Empty;
    }

    private bool MatchesWindow(IntPtr handle, string title, string className, int? processId, TextMatchMode matchMode)
    {
        if (!string.IsNullOrEmpty(className) && !Matches(GetClassName(handle), className, matchMode))
        {
            return false;
        }

        if (processId.HasValue)
        {
            _nativeMethods.GetWindowThreadProcessId(handle, out var actualProcessId);
            if ((int)actualProcessId != processId.Value)
            {
                return false;
            }
        }

        return string.IsNullOrEmpty(title) || Matches(GetText(handle), title, matchMode);
    }

    private bool MatchesControl(IntPtr handle, string text, string className, int? controlId, TextMatchMode matchMode)
    {
        if (!string.IsNullOrEmpty(className) && !Matches(GetClassName(handle), className, matchMode))
        {
            return false;
        }

        if (controlId.HasValue && _nativeMethods.GetDlgCtrlId(handle) != controlId.Value)
        {
            return false;
        }

        return string.IsNullOrEmpty(text) || Matches(GetText(handle), text, matchMode);
    }

    /// <summary>
    /// Retries the search until it succeeds or the timeout elapses, so flows can wait for a window to appear.
    /// </summary>
    private IntPtr FindFirst(Func<IReadOnlyList<IntPtr>> candidateFactory, Func<IntPtr, bool> predicate, int timeoutMilliseconds)
    {
        var deadline = _clock.ElapsedMilliseconds + timeoutMilliseconds;

        while (true)
        {
            foreach (var handle in candidateFactory())
            {
                if (predicate(handle))
                {
                    return handle;
                }
            }

            if (_clock.ElapsedMilliseconds >= deadline)
            {
                return IntPtr.Zero;
            }

            _clock.Sleep(PollIntervalMilliseconds);
        }
    }
}
