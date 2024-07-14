// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.Win32;
using PowerAutomate.Desktop.Windows.Registry.Abstractions;

namespace PowerAutomate.Desktop.Windows.Registry.Win32;

internal sealed class Win32RegistryHive : Win32RegistryKeyItem, IRegistryHive
{
    internal Win32RegistryHive(IRegistry registry, RegistryKey registryKey, string name)
        : base(registry, registryKey, name)
    {
    }
}