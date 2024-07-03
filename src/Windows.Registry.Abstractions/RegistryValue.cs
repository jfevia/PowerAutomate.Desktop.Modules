// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace PowerAutomate.Desktop.Windows.Registry.Abstractions;

public interface IRegistryValue : IRegistryItem
{
    object Data { get; }
}