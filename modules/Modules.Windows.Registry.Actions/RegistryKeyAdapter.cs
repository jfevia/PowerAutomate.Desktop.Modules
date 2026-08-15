// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

// Pure registry adapter; all members forward to Microsoft.Win32.RegistryKey.
[ExcludeFromCodeCoverage]
internal sealed class RegistryKeyAdapter : IRegistryKey
{
    private readonly RegistryKey _registryKey;

    public RegistryKeyAdapter(RegistryKey registryKey)
    {
        _registryKey = registryKey;
    }

    public string Name => _registryKey.Name;
    public IRegistryKey CreateSubKey(string name) => new RegistryKeyAdapter(_registryKey.CreateSubKey(name));
    public void DeleteSubKey(string name, bool throwOnMissingSubKey) => _registryKey.DeleteSubKey(name, throwOnMissingSubKey);
    public void DeleteValue(string name, bool throwOnMissingValue) => _registryKey.DeleteValue(name, throwOnMissingValue);
    public void Dispose() => _registryKey.Dispose();
    public IEnumerable<string> GetSubKeyNames() => _registryKey.GetSubKeyNames();
    public object? GetValue(string name, object? defaultValue, bool expandEnvironmentVariables) => _registryKey.GetValue(name, defaultValue, expandEnvironmentVariables ? RegistryValueOptions.None : RegistryValueOptions.DoNotExpandEnvironmentNames);
    public RegistryValueKind GetValueKind(string name) => FromNative(_registryKey.GetValueKind(name));
    public IEnumerable<string> GetValueNames() => _registryKey.GetValueNames();
    public IRegistryKey? OpenSubKey(string name, bool writable) => _registryKey.OpenSubKey(name, writable) is { } key ? new RegistryKeyAdapter(key) : null;
    public void SetValue(string name, object? value, RegistryValueKind kind) => _registryKey.SetValue(name, value, kind.ToNative());

    private static RegistryValueKind FromNative(Microsoft.Win32.RegistryValueKind kind) => kind switch
    {
        Microsoft.Win32.RegistryValueKind.String => RegistryValueKind.String,
        Microsoft.Win32.RegistryValueKind.ExpandString => RegistryValueKind.ExpandString,
        Microsoft.Win32.RegistryValueKind.MultiString => RegistryValueKind.MultiString,
        Microsoft.Win32.RegistryValueKind.DWord => RegistryValueKind.DWord,
        Microsoft.Win32.RegistryValueKind.QWord => RegistryValueKind.QWord,
        Microsoft.Win32.RegistryValueKind.Binary => RegistryValueKind.Binary,
        _ => RegistryValueKind.String
    };
}
