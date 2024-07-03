// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using Microsoft.Win32;

namespace PowerAutomate.Desktop.Windows.Registry.Win32;

internal static class RegistryKeyPermissionCheckExtensions
{
    public static RegistryKeyPermissionCheck ToWin32(this Abstractions.RegistryKeyPermissionCheck permissionCheck)
    {
        return permissionCheck switch
        {
            Abstractions.RegistryKeyPermissionCheck.Default => RegistryKeyPermissionCheck.Default,
            Abstractions.RegistryKeyPermissionCheck.ReadSubTree => RegistryKeyPermissionCheck.ReadSubTree,
            Abstractions.RegistryKeyPermissionCheck.ReadWriteSubTree => RegistryKeyPermissionCheck.ReadWriteSubTree,
            _ => throw new ArgumentOutOfRangeException(nameof(permissionCheck), permissionCheck, null)
        };
    }
}