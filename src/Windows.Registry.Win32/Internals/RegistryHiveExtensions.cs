// --------------------------------------------------------------
// Copyright (c) Power Automate for desktop. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.Win32;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Win32.Internals;

internal static class RegistryHiveExtensions
{
    public static RegistryHive ToWin32(this Abstractions.RegistryHive registryHive)
    {
        return registryHive switch
        {
            Abstractions.RegistryHive.ClassesRoot => RegistryHive.ClassesRoot,
            Abstractions.RegistryHive.CurrentUser => RegistryHive.CurrentUser,
            Abstractions.RegistryHive.LocalMachine => RegistryHive.LocalMachine,
            Abstractions.RegistryHive.Users => RegistryHive.Users,
            Abstractions.RegistryHive.PerformanceData => RegistryHive.PerformanceData,
            Abstractions.RegistryHive.CurrentConfig => RegistryHive.CurrentConfig,
            _ => throw new ArgumentOutOfRangeException(nameof(registryHive), registryHive, null)
        };
    }
}