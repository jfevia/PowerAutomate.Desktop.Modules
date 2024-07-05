// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using Microsoft.Win32;
using PowerAutomate.Desktop.Windows.Registry.Abstractions;

namespace PowerAutomate.Desktop.Windows.Registry.Win32;

internal sealed class Win32RegistryKey : Win32RegistryKeyItem, IRegistryKey
{
    internal Win32RegistryKey(IRegistry registry, RegistryKey registryKey, string name)
        : base(registry, registryKey, name)
    {
    }
}