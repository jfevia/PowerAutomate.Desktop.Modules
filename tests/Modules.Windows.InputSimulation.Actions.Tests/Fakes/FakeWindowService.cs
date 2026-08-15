// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Enums;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Services;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Types;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

internal sealed class FakeWindowService : IWindowService
{
    public int CenterX { get; set; }
    public int CenterY { get; set; }
    public IReadOnlyList<IntPtr> Children { get; set; } = Array.Empty<IntPtr>();
    public string ClassNameResult { get; set; } = string.Empty;
    public int ControlIdResult { get; set; }
    public string CriteriaDescription { get; set; } = "the supplied criteria";
    public IntPtr FoundControl { get; set; }
    public IntPtr FoundWindow { get; set; }
    public bool MatchesResult { get; set; } = true;
    public IntPtr ParentResult { get; set; }
    public int ScreenX { get; set; }
    public int ScreenY { get; set; }
    public string TextResult { get; set; } = string.Empty;
    public IReadOnlyList<IntPtr> TopLevel { get; set; } = Array.Empty<IntPtr>();

    public bool? RequestedRecursive { get; private set; }
    public IntPtr RequestedParent { get; private set; }

    public void ClientToScreen(IntPtr handle, int x, int y, out int screenX, out int screenY)
    {
        screenX = ScreenX;
        screenY = ScreenY;
    }

    public string DescribeCriteria(params SearchCriterion[] criteria) => CriteriaDescription;

    public IReadOnlyList<IntPtr> EnumerateChildWindows(IntPtr parent, bool recursive)
    {
        RequestedParent = parent;
        RequestedRecursive = recursive;
        return Children;
    }

    public IReadOnlyList<IntPtr> EnumerateTopLevelWindows() => TopLevel;

    public IntPtr FindControl(IntPtr parent, string text, string className, int? controlId, TextMatchMode matchMode, bool recursive, int timeoutMilliseconds)
    {
        RequestedParent = parent;
        RequestedRecursive = recursive;
        return FoundControl;
    }

    public IntPtr FindWindow(string title, string className, int? processId, TextMatchMode matchMode, int timeoutMilliseconds) => FoundWindow;

    public void GetCenter(IntPtr handle, out int x, out int y)
    {
        x = CenterX;
        y = CenterY;
    }

    public string GetClassName(IntPtr handle) => ClassNameResult;

    public int GetControlId(IntPtr handle) => ControlIdResult;

    public IntPtr GetParent(IntPtr handle) => ParentResult;

    public string GetText(IntPtr handle) => TextResult;

    public bool Matches(string candidate, string pattern, TextMatchMode matchMode) => MatchesResult;

    public WindowObject ToWindowObject(IntPtr handle) =>
        new(handle.ToInt64(), ClassNameResult, TextResult, ControlIdResult, 0, true, true, 0, 0);
}
