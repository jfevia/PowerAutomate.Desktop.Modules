// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

public static class RegistryValueKindExtensions
{
    public static bool CanExpandEnvironmentVariables(this RegistryValueKind value) => value is RegistryValueKind.ExpandString;

    public static Microsoft.Win32.RegistryValueKind ToNative(this RegistryValueKind value)
    {
        return value switch
        {
            RegistryValueKind.String => Microsoft.Win32.RegistryValueKind.String,
            RegistryValueKind.ExpandString => Microsoft.Win32.RegistryValueKind.ExpandString,
            RegistryValueKind.Binary => Microsoft.Win32.RegistryValueKind.Binary,
            RegistryValueKind.DWord => Microsoft.Win32.RegistryValueKind.DWord,
            RegistryValueKind.MultiString => Microsoft.Win32.RegistryValueKind.MultiString,
            RegistryValueKind.QWord => Microsoft.Win32.RegistryValueKind.QWord,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
