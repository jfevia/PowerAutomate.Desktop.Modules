// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Linq;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

public static class RegistryExtensions
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

    public static IRegistryHive ParseHive(this IRegistry registry, string name)
    {
        foreach (var registryHive in registry.GetHives())
        {
            if (registryHive.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                return registryHive;
            }
        }

        throw new InvalidOperationException("Could not parse registry hive");
    }

    public static IRegistryKey ParseKey(this IRegistry registry, string path)
    {
        var items = path.Split(['\\'], StringSplitOptions.RemoveEmptyEntries);
        using var registryHive = registry.ParseHive(items.First());
        IRegistryKey? registryKey = null;

        foreach (var name in items.Skip(1))
        {
            if (registryKey is null)
            {
                registryKey = registryHive.OpenSubKey(name);
            }
            else
            {
                using var previousRegistryKey = registryKey;
                registryKey = previousRegistryKey.OpenSubKey(name);
            }
        }

        return registryKey ?? throw new InvalidOperationException("Could not parse registry key");
    }
}