// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.WindowsRegistry.Actions;

internal static class RegistryValueKindExtensions
{
    public static Windows.Registry.Abstractions.RegistryValueKind ToAbstractions(this RegistryValueKind value)
    {
        return value switch
        {
            RegistryValueKind.String => Windows.Registry.Abstractions.RegistryValueKind.String,
            RegistryValueKind.ExpandString => Windows.Registry.Abstractions.RegistryValueKind.ExpandString,
            RegistryValueKind.Binary => Windows.Registry.Abstractions.RegistryValueKind.Binary,
            RegistryValueKind.DWord => Windows.Registry.Abstractions.RegistryValueKind.DWord,
            RegistryValueKind.MultiString => Windows.Registry.Abstractions.RegistryValueKind.MultiString,
            RegistryValueKind.QWord => Windows.Registry.Abstractions.RegistryValueKind.QWord,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}