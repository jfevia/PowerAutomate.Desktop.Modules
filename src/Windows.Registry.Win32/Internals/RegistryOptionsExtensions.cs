// --------------------------------------------------------------
// Copyright (c) Power Automate for desktop. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Win32;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Win32.Internals;

internal static class RegistryOptionsExtensions
{
    public static RegistryOptions ToWin32(this Abstractions.RegistryOptions options)
    {
        var result = RegistryOptions.None;

        if (options.HasFlag(Abstractions.RegistryOptions.Volatile)) result |= RegistryOptions.Volatile;

        return result;
    }
}