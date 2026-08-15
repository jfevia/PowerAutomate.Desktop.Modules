// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

public interface IRegistryKey : IDisposable
{
    string Name { get; }
    IRegistryKey CreateSubKey(string name);
    void DeleteSubKey(string name, bool throwOnMissingSubKey);
    void DeleteValue(string name, bool throwOnMissingValue);
    IRegistryKey? OpenSubKey(string name, bool writable);
    IEnumerable<string> GetSubKeyNames();
    object? GetValue(string name, object? defaultValue, bool expandEnvironmentVariables);
    RegistryValueKind GetValueKind(string name);
    IEnumerable<string> GetValueNames();
    void SetValue(string name, object? value, RegistryValueKind kind);
}
