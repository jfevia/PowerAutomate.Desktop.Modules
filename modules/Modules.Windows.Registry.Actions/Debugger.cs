// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Diagnostics;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

internal static class Debugger
{
    [Conditional("DEBUG")]
    public static void Launch()
    {
        System.Diagnostics.Debugger.Launch();
    }
}