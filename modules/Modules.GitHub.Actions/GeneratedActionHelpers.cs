// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Globalization;

namespace PowerAutomate.Desktop.Modules.GitHub.Actions;

public static class GeneratedActionHelpers
{
    public static string Escape(object? value)
    {
        return Uri.EscapeDataString(Convert.ToString(value, CultureInfo.InvariantCulture));
    }
}