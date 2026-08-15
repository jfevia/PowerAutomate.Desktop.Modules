// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

internal sealed class RecordedSetValue
{
    public RecordedSetValue(string name, object? value, RegistryValueKind kind)
    {
        Name = name;
        Value = value;
        Kind = kind;
    }

    public RegistryValueKind Kind { get; }
    public string Name { get; }
    public object? Value { get; }
}