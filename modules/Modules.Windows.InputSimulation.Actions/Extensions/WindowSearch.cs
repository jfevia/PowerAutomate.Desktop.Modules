// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Extensions;

internal static class WindowSearch
{
    private const int PollIntervalMilliseconds = 100;

    /// <summary>
    /// Retries the search until it succeeds or the timeout elapses, so flows can wait for a window to appear.
    /// </summary>
    public static IntPtr FindFirst(Func<IEnumerable<IntPtr>> candidateFactory, Func<IntPtr, bool> predicate, int timeoutMilliseconds)
    {
        var stopwatch = Stopwatch.StartNew();

        while (true)
        {
            foreach (var handle in candidateFactory())
            {
                if (predicate(handle))
                {
                    return handle;
                }
            }

            if (stopwatch.ElapsedMilliseconds >= timeoutMilliseconds)
            {
                return IntPtr.Zero;
            }

            Thread.Sleep(PollIntervalMilliseconds);
        }
    }

    public static bool MatchesWindow(IntPtr handle, string? title, string? className, int? processId, TextMatchMode matchMode)
    {
        if (!string.IsNullOrEmpty(className) && !WindowExtensions.Matches(WindowExtensions.GetClassName(handle), className, matchMode))
        {
            return false;
        }

        if (processId.HasValue)
        {
            NativeMethods.GetWindowThreadProcessId(handle, out var actualProcessId);
            if ((int)actualProcessId != processId.Value)
            {
                return false;
            }
        }

        return string.IsNullOrEmpty(title) || WindowExtensions.Matches(WindowExtensions.GetText(handle, 200), title, matchMode);
    }

    public static bool MatchesControl(IntPtr handle, string? text, string? className, int? controlId, TextMatchMode matchMode)
    {
        if (!string.IsNullOrEmpty(className) && !WindowExtensions.Matches(WindowExtensions.GetClassName(handle), className, matchMode))
        {
            return false;
        }

        if (controlId.HasValue && NativeMethods.GetDlgCtrlID(handle) != controlId.Value)
        {
            return false;
        }

        return string.IsNullOrEmpty(text) || WindowExtensions.Matches(WindowExtensions.GetText(handle, 200), text, matchMode);
    }

    /// <summary>
    /// Builds a human readable description of the search terms for error messages.
    /// </summary>
    public static string DescribeCriteria(params (string Name, object? Value)[] criteria)
    {
        var parts = new List<string>();

        foreach (var (name, value) in criteria)
        {
            if (value is null || value is string text && string.IsNullOrEmpty(text))
            {
                continue;
            }

            parts.Add($"{name} '{value}'");
        }

        return parts.Count == 0 ? "the supplied criteria" : string.Join(", ", parts);
    }
}
