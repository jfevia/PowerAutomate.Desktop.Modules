// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using PowerAutomate.Desktop.Windows.Registry.Abstractions;

namespace PowerAutomate.Desktop.Windows.Registry.Win32;

internal static class RegistryValueKindExtensions
{
    public static RegistryValueKind ToAbstraction(this Microsoft.Win32.RegistryValueKind valueKind)
    {
        return valueKind switch
        {
            Microsoft.Win32.RegistryValueKind.String => RegistryValueKind.String,
            Microsoft.Win32.RegistryValueKind.ExpandString => RegistryValueKind.ExpandString,
            Microsoft.Win32.RegistryValueKind.Binary => RegistryValueKind.Binary,
            Microsoft.Win32.RegistryValueKind.DWord => RegistryValueKind.DWord,
            Microsoft.Win32.RegistryValueKind.MultiString => RegistryValueKind.MultiString,
            Microsoft.Win32.RegistryValueKind.QWord => RegistryValueKind.QWord,
            Microsoft.Win32.RegistryValueKind.Unknown => RegistryValueKind.Unknown,
            Microsoft.Win32.RegistryValueKind.None => RegistryValueKind.None,
            _ => throw new ArgumentOutOfRangeException(nameof(valueKind), valueKind, null)
        };
    }

    public static Microsoft.Win32.RegistryValueKind ToWin32(this RegistryValueKind valueKind)
    {
        return valueKind switch
        {
            RegistryValueKind.String => Microsoft.Win32.RegistryValueKind.String,
            RegistryValueKind.ExpandString => Microsoft.Win32.RegistryValueKind.ExpandString,
            RegistryValueKind.Binary => Microsoft.Win32.RegistryValueKind.Binary,
            RegistryValueKind.DWord => Microsoft.Win32.RegistryValueKind.DWord,
            RegistryValueKind.MultiString => Microsoft.Win32.RegistryValueKind.MultiString,
            RegistryValueKind.QWord => Microsoft.Win32.RegistryValueKind.QWord,
            RegistryValueKind.Unknown => Microsoft.Win32.RegistryValueKind.Unknown,
            RegistryValueKind.None => Microsoft.Win32.RegistryValueKind.None,
            _ => throw new ArgumentOutOfRangeException(nameof(valueKind), valueKind, null)
        };
    }
}