// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using PowerAutomate.Desktop.Windows.Registry.Abstractions;
using RegistryKeyPermissionCheck = PowerAutomate.Desktop.Windows.Registry.Abstractions.RegistryKeyPermissionCheck;
using RegistryOptions = PowerAutomate.Desktop.Windows.Registry.Abstractions.RegistryOptions;
using RegistryValueKind = PowerAutomate.Desktop.Windows.Registry.Abstractions.RegistryValueKind;
using RegistryValueOptions = PowerAutomate.Desktop.Windows.Registry.Abstractions.RegistryValueOptions;

namespace PowerAutomate.Desktop.Windows.Registry.Win32;

internal abstract class Win32RegistryKeyItem : Win32RegistryItem, IRegistryKeyItem
{
    private readonly IRegistry _registry;
    private readonly RegistryKey _registryKey;

    protected internal Win32RegistryKeyItem(IRegistry registry, RegistryKey registryKey, string name)
        : base(name)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _registryKey = registryKey ?? throw new ArgumentNullException(nameof(registryKey));
    }

    public SafeHandle Handle => _registryKey.Handle;

    public IRegistryKey? CreateSubKey(string subKey)
    {
        var registrySubKey = _registryKey.CreateSubKey(subKey);
        return registrySubKey is not null ? new Win32RegistryKey(_registry, registrySubKey, registrySubKey.Name) : null;
    }

    public IRegistryKey CreateSubKey(string subKey, bool writable)
    {
        var registrySubKey = _registryKey.CreateSubKey(subKey, writable);
        return new Win32RegistryKey(_registry, registrySubKey, registrySubKey.Name);
    }

    public IRegistryKey CreateSubKey(string subKey, bool writable, RegistryOptions options)
    {
        var registrySubKey = _registryKey.CreateSubKey(subKey, writable, options.ToWin32());
        return new Win32RegistryKey(_registry, registrySubKey, registrySubKey.Name);
    }

    public IRegistryKey? CreateSubKey(string subKey, RegistryKeyPermissionCheck permissionCheck)
    {
        var registrySubKey = _registryKey.CreateSubKey(subKey, permissionCheck.ToWin32());
        return registrySubKey is not null ? new Win32RegistryKey(_registry, registrySubKey, registrySubKey.Name) : null;
    }

    public IRegistryKey? CreateSubKey(string subKey, RegistryKeyPermissionCheck permissionCheck,
        RegistryOptions options)
    {
        var registrySubKey = _registryKey.CreateSubKey(subKey, permissionCheck.ToWin32(), options.ToWin32());
        return registrySubKey is not null ? new Win32RegistryKey(_registry, registrySubKey, registrySubKey.Name) : null;
    }

    public void DeleteSubKey(string subKey)
    {
        _registryKey.DeleteSubKey(subKey);
    }

    public void DeleteSubKey(string subKey, bool throwOnMissingSubKey)
    {
        _registryKey.DeleteSubKey(subKey, throwOnMissingSubKey);
    }

    public void DeleteSubKeyTree(string subKey)
    {
        _registryKey.DeleteSubKeyTree(subKey);
    }

    public void DeleteSubKeyTree(string subKey, bool throwOnMissingSubKey)
    {
        _registryKey.DeleteSubKeyTree(subKey, throwOnMissingSubKey);
    }

    public void DeleteValue(string name)
    {
        _registryKey.DeleteValue(name);
    }

    public void DeleteValue(string name, bool throwOnMissingValue)
    {
        _registryKey.DeleteValue(name, throwOnMissingValue);
    }

    public IEnumerable<IRegistryKey> GetSubKeys()
    {
        var registryKeys = _registryKey.GetSubKeyNames()
                                       .Select(subKeyName => _registryKey.OpenSubKey(subKeyName, false));
        return registryKeys.Where(registryKey => registryKey is not null)
                           .Select(registryKey =>
                           {
                               var registryKeyName = registryKey!.Name.Substring(registryKey.Name.LastIndexOf('\\') + 1);
                               return new Win32RegistryKey(_registry, registryKey, registryKeyName);
                           });
    }

    public object GetValue(string name)
    {
        return _registryKey.GetValue(name);
    }

    public object GetValue(string name, object defaultValue)
    {
        return _registryKey.GetValue(name, defaultValue);
    }

    public object GetValue(string name, object defaultValue, RegistryValueOptions options)
    {
        return _registryKey.GetValue(name, defaultValue, options.ToWin32());
    }

    public RegistryValueKind GetValueKind(string name)
    {
        return _registryKey.GetValueKind(name).ToAbstraction();
    }

    public IEnumerable<IRegistryValue> GetValues()
    {
        return _registryKey.GetValueNames().Select(valueName =>
            new Win32RegistryValue(valueName, () => _registryKey.GetValue(valueName)));
    }

    public IRegistryKey? OpenSubKey(string name)
    {
        var registryKey = _registryKey.OpenSubKey(name, false);
        if (registryKey is null) return null;

        var registryKeyName = registryKey.Name.Substring(registryKey.Name.LastIndexOf('\\') + 1);
        return new Win32RegistryKey(_registry, registryKey, registryKeyName);
    }

    public void SetValue(string name, object value)
    {
        _registryKey.SetValue(name, value);
    }

    public void SetValue(string name, object value, RegistryValueKind valueKind)
    {
        _registryKey.SetValue(name, value, valueKind.ToWin32());
    }

    public override void Accept(IRegistryVisitor registryVisitor)
    {
        foreach (var registryKey in GetSubKeys())
        {
            if (!registryVisitor.VisitEnterRegistryKey(registryKey)) return;
            registryVisitor.VisitRegistryKey(registryKey);
            registryVisitor.VisitLeaveRegistryKey(registryKey);
        }

        foreach (var registryValue in GetValues())
        {
            registryVisitor.VisitRegistryValue(registryValue);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _registryKey.Dispose();
        }

        base.Dispose(disposing);
    }
}