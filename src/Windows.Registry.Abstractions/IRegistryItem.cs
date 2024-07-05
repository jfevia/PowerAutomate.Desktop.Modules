// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace PowerAutomate.Desktop.Windows.Registry.Abstractions;

public interface IRegistryItem : IRegistryVisitable
{
    string Name { get; }
}