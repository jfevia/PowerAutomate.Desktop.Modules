// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Services;

/// <summary>
/// Locates windows and controls and reads the state the actions expose.
/// </summary>
public interface IWindowService
{
    void ClientToScreen(IntPtr handle, int x, int y, out int screenX, out int screenY);

    string DescribeCriteria(params SearchCriterion[] criteria);

    IReadOnlyList<IntPtr> EnumerateChildWindows(IntPtr parent, bool recursive);

    IReadOnlyList<IntPtr> EnumerateTopLevelWindows();

    IntPtr FindControl(IntPtr parent, string text, string className, int? controlId, TextMatchMode matchMode, bool recursive, int timeoutMilliseconds);

    IntPtr FindWindow(string title, string className, int? processId, TextMatchMode matchMode, int timeoutMilliseconds);

    void GetCenter(IntPtr handle, out int x, out int y);

    string GetClassName(IntPtr handle);

    int GetControlId(IntPtr handle);

    IntPtr GetParent(IntPtr handle);

    string GetText(IntPtr handle);

    bool Matches(string candidate, string pattern, TextMatchMode matchMode);

    WindowObject ToWindowObject(IntPtr handle);
}
