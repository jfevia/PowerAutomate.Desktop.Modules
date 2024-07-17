// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

public static class RegistryExtensions
{
    private static IEnumerable<RegistryKey> GetHives()
    {
        yield return Microsoft.Win32.Registry.ClassesRoot;
        yield return Microsoft.Win32.Registry.CurrentConfig;
        yield return Microsoft.Win32.Registry.CurrentUser;
        yield return Microsoft.Win32.Registry.PerformanceData;
        yield return Microsoft.Win32.Registry.LocalMachine;
        yield return Microsoft.Win32.Registry.Users;
    }

    public static RegistryKey ParseHive(string name)
    {
        foreach (var registryHive in GetHives())
        {
            if (registryHive.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                return registryHive;
            }
        }

        throw new InvalidOperationException("Could not parse registry hive");
    }

    public static RegistryKey ParseKey(string path, bool writable)
    {
        var items = path.Split(['\\'], StringSplitOptions.RemoveEmptyEntries);
        using var registryHive = ParseHive(items.First());
        RegistryKey? registryKey = null;

        foreach (var name in items.Skip(1))
        {
            if (registryKey is null)
            {
                registryKey = registryHive.OpenSubKey(name, writable);
            }
            else
            {
                using var previousRegistryKey = registryKey;
                registryKey = previousRegistryKey.OpenSubKey(name, writable);
            }
        }

        return registryKey ?? throw new InvalidOperationException("Could not parse registry key");
    }
}