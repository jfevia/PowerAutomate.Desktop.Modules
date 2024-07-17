// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

internal static class RegistryValueKindExtensions
{
    public static bool CanExpandEnvironmentVariables(this RegistryValueKind value)
    {
        return value is RegistryValueKind.ExpandString;
    }


    public static Microsoft.Win32.RegistryValueKind ToAbstractions(this RegistryValueKind value)
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

    public static RegistryValueKind ToNative(this Microsoft.Win32.RegistryValueKind value)
    {
        return value switch
        {
            Microsoft.Win32.RegistryValueKind.String => RegistryValueKind.String,
            Microsoft.Win32.RegistryValueKind.ExpandString => RegistryValueKind.ExpandString,
            Microsoft.Win32.RegistryValueKind.Binary => RegistryValueKind.Binary,
            Microsoft.Win32.RegistryValueKind.DWord => RegistryValueKind.DWord,
            Microsoft.Win32.RegistryValueKind.MultiString => RegistryValueKind.MultiString,
            Microsoft.Win32.RegistryValueKind.QWord => RegistryValueKind.QWord,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}