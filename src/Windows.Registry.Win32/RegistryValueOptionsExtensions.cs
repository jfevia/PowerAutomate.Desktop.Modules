// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Win32;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Win32;

internal static class RegistryValueOptionsExtensions
{
    public static RegistryValueOptions ToWin32(this Abstractions.RegistryValueOptions options)
    {
        var result = RegistryValueOptions.None;

        if (options.HasFlag(Abstractions.RegistryValueOptions.DoNotExpandEnvironmentNames))
        {
            result |= RegistryValueOptions.DoNotExpandEnvironmentNames;
        }

        return result;
    }
}