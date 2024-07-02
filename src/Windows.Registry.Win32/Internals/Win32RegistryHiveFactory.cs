// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.Win32;
using PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;
using RegistryHive = PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions.RegistryHive;
using RegistryView = PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions.RegistryView;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Win32.Internals;

internal sealed class Win32RegistryHiveFactory : IRegistryHiveFactory
{
    private readonly IRegistry _registry;

    internal Win32RegistryHiveFactory(IRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public IRegistryHive OpenBaseKey(RegistryHive registryHive)
    {
        return _registry.OpenBaseKey(registryHive);
    }

    public IRegistryHive OpenBaseKey(RegistryHive registryHive, RegistryView registryView)
    {
        var registryKey = RegistryKey.OpenBaseKey(registryHive.ToWin32(), registryView.ToWin32());
        return new Win32RegistryHive(_registry, registryKey, registryKey.Name);
    }

    public IRegistryHive OpenRemoteBaseKey(RegistryHive registryHive, string machineName)
    {
        var registryKey = RegistryKey.OpenRemoteBaseKey(registryHive.ToWin32(), machineName);
        return new Win32RegistryHive(_registry, registryKey, registryKey.Name);
    }

    public IRegistryHive OpenRemoteBaseKey(RegistryHive registryHive, string machineName, RegistryView registryView)
    {
        var registryKey = RegistryKey.OpenRemoteBaseKey(registryHive.ToWin32(), machineName, registryView.ToWin32());
        return new Win32RegistryHive(_registry, registryKey, registryKey.Name);
    }
}