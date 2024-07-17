// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace PowerAutomate.Desktop.Windows.Registry.Abstractions;

public interface IRegistryKeyItem : IRegistryItem
{
    IRegistryKey? CreateSubKey(string subKey);
    IRegistryKey? CreateSubKey(string subKey, bool writable);
    IRegistryKey? CreateSubKey(string subKey, bool writable, RegistryOptions options);
    IRegistryKey? CreateSubKey(string subKey, RegistryKeyPermissionCheck permissionCheck);
    IRegistryKey? CreateSubKey(string subKey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions options);
    void DeleteSubKey(string subKey);
    void DeleteSubKey(string subKey, bool throwOnMissingSubKey);
    void DeleteSubKeyTree(string subKey);
    void DeleteSubKeyTree(string subKey, bool throwOnMissingSubKey);
    void DeleteValue(string name);
    void DeleteValue(string name, bool throwOnMissingValue);
    IEnumerable<IRegistryKey> GetSubKeys();
    object GetValue(string name);
    object GetValue(string name, object defaultValue);
    object GetValue(string name, object defaultValue, RegistryValueOptions options);
    RegistryValueKind GetValueKind(string name);
    IEnumerable<IRegistryValue> GetValues();
    IRegistryKey? OpenSubKey(string name);
    IRegistryKey? OpenSubKey(string name, bool writable);
    IRegistryKey? OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck);
    IRegistryKey? OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights registryRights);
    IRegistryKey? OpenSubKey(string name, RegistryRights registryRights);
    void SetValue(string name, object value);
    void SetValue(string name, object value, RegistryValueKind valueKind);
    SafeHandle Handle { get; }
}