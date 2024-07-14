// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

internal static class RegistryValueKindExtensions
{
    public static bool CanExpandEnvironmentVariables(this Desktop.Windows.Registry.Abstractions.RegistryValueKind value)
    {
        return value is Desktop.Windows.Registry.Abstractions.RegistryValueKind.ExpandString;
    }

    public static Desktop.Windows.Registry.Abstractions.RegistryValueKind ToAbstractions(this RegistryValueKind value)
    {
        return value switch
        {
            RegistryValueKind.String => Desktop.Windows.Registry.Abstractions.RegistryValueKind.String,
            RegistryValueKind.ExpandString => Desktop.Windows.Registry.Abstractions.RegistryValueKind.ExpandString,
            RegistryValueKind.Binary => Desktop.Windows.Registry.Abstractions.RegistryValueKind.Binary,
            RegistryValueKind.DWord => Desktop.Windows.Registry.Abstractions.RegistryValueKind.DWord,
            RegistryValueKind.MultiString => Desktop.Windows.Registry.Abstractions.RegistryValueKind.MultiString,
            RegistryValueKind.QWord => Desktop.Windows.Registry.Abstractions.RegistryValueKind.QWord,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}