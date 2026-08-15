// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

public static class RegistryExtensions
{
    public static IRegistryKey ParseHive(string name, IEnumerable<IRegistryKey> hives)
    {
        foreach (var registryHive in hives)
        {
            if (registryHive.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                return registryHive;
            }
        }

        throw new InvalidOperationException("Could not parse registry hive");
    }

    public static IRegistryKey ParseKey(string path, bool writable, IEnumerable<IRegistryKey> hives)
    {
        var items = path.Split(['\\'], StringSplitOptions.RemoveEmptyEntries);
        var registryHive = ParseHive(items.First(), hives);
        IRegistryKey? registryKey = null;

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
