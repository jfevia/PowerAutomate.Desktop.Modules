// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.Win32;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Win32;

internal static class RegistryViewExtensions
{
    public static RegistryView ToWin32(this Abstractions.RegistryView registryView)
    {
        return registryView switch
        {
            Abstractions.RegistryView.Default => RegistryView.Default,
            Abstractions.RegistryView.Registry64 => RegistryView.Registry64,
            Abstractions.RegistryView.Registry32 => RegistryView.Registry32,
            _ => throw new ArgumentOutOfRangeException(nameof(registryView), registryView, null)
        };
    }
}