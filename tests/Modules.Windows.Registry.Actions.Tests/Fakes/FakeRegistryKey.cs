// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

internal sealed class FakeRegistryKey : IRegistryKey
{
    public FakeRegistryKey(string name)
    {
        Name = name;
    }

    public List<(string Name, bool ThrowOnMissing)> DeletedSubKeys { get; } = new();
    public List<(string Name, bool ThrowOnMissing)> DeletedValues { get; } = new();
    public int DisposeCount { get; private set; }
    public Exception? ExceptionToThrow { get; set; }
    public string Name { get; }
    public List<(string Name, bool Writable)> OpenedSubKeys { get; } = new();
    public Dictionary<string, FakeRegistryKey?> SubKeys { get; } = new();
    public List<RecordedSetValue> SetValues { get; } = new();
    public Dictionary<string, (object? Value, RegistryValueKind Kind)> Values { get; } = new();
    public List<(string Name, object? DefaultValue, bool Expand)> ValueReads { get; } = new();

    public IRegistryKey CreateSubKey(string name)
    {
        ThrowIfNeeded();
        var key = new FakeRegistryKey(name);
        SubKeys[name] = key;
        return key;
    }

    public void DeleteSubKey(string name, bool throwOnMissingSubKey)
    {
        ThrowIfNeeded();
        DeletedSubKeys.Add((name, throwOnMissingSubKey));
    }

    public void DeleteValue(string name, bool throwOnMissingValue)
    {
        ThrowIfNeeded();
        DeletedValues.Add((name, throwOnMissingValue));
    }

    public void Dispose() => DisposeCount++;

    public IEnumerable<string> GetSubKeyNames()
    {
        ThrowIfNeeded();
        return SubKeys.Keys.ToArray();
    }

    public object? GetValue(string name, object? defaultValue, bool expandEnvironmentVariables)
    {
        ThrowIfNeeded();
        ValueReads.Add((name, defaultValue, expandEnvironmentVariables));
        return Values.TryGetValue(name, out var value) ? value.Value : defaultValue;
    }

    public RegistryValueKind GetValueKind(string name)
    {
        ThrowIfNeeded();
        return Values[name].Kind;
    }

    public IEnumerable<string> GetValueNames()
    {
        ThrowIfNeeded();
        return Values.Keys.ToArray();
    }

    public IRegistryKey? OpenSubKey(string name, bool writable)
    {
        ThrowIfNeeded();
        OpenedSubKeys.Add((name, writable));
        return SubKeys.TryGetValue(name, out var key) ? key : null;
    }

    public void SetValue(string name, object? value, RegistryValueKind kind)
    {
        ThrowIfNeeded();
        SetValues.Add(new RecordedSetValue(name, value, kind));
        Values[name] = (value, kind);
    }

    private void ThrowIfNeeded()
    {
        if (ExceptionToThrow is not null)
        {
            throw ExceptionToThrow;
        }
    }
}
