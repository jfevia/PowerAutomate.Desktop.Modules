// --------------------------------------------------------------
// Copyright (c) Power Automate for desktop. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Win32;
using PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Win32.Internals;

internal sealed class Win32RegistryHive : Win32RegistryKeyItem, IRegistryHive
{
    internal Win32RegistryHive(IRegistry registry, RegistryKey registryKey, string name)
        : base(registry, registryKey, name)
    {
    }
}