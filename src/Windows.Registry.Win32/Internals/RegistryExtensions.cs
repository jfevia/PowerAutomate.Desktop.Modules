// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Win32.Internals;

internal static class RegistryExtensions
{
    public static IRegistryHive OpenBaseKey(this IRegistry registry, RegistryHive registryHive)
    {
        return registryHive switch
        {
            RegistryHive.ClassesRoot => registry.ClassesRoot,
            RegistryHive.CurrentUser => registry.CurrentUser,
            RegistryHive.LocalMachine => registry.LocalMachine,
            RegistryHive.Users => registry.Users,
            RegistryHive.PerformanceData => registry.PerformanceData,
            RegistryHive.CurrentConfig => registry.CurrentConfig,
            _ => throw new ArgumentOutOfRangeException(nameof(registryHive), registryHive, null)
        };
    }
}