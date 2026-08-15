// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;

/// <summary>
/// Combo boxes and list boxes select the same way, differing only in their message numbers.
/// </summary>
internal static class ListSelection
{
    public static int Resolve(
        InputSimulationContext context,
        IntPtr handle,
        int? index,
        string text,
        uint findMessage,
        uint selectMessage,
        int errorResult,
        string controlDescription)
    {
        var resolvedIndex = ResolveIndex(context, handle, index, text, findMessage, errorResult, controlDescription);

        var result = context.MessageDispatcher.Send(handle, selectMessage, (IntPtr)resolvedIndex, IntPtr.Zero).ToInt32();
        if (result == errorResult)
        {
            throw new ControlNotFoundException($"index {resolvedIndex} in the {controlDescription}");
        }

        return resolvedIndex;
    }

    private static int ResolveIndex(
        InputSimulationContext context,
        IntPtr handle,
        int? index,
        string text,
        uint findMessage,
        int errorResult,
        string controlDescription)
    {
        if (index.HasValue)
        {
            return index.Value;
        }

        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Provide either the item text or the item index to select.", nameof(text));
        }

        var found = context.MessageDispatcher.SendText(handle, findMessage, (IntPtr)(-1), text).ToInt32();
        if (found == errorResult)
        {
            throw new ControlNotFoundException($"item '{text}' in the {controlDescription}");
        }

        return found;
    }
}
