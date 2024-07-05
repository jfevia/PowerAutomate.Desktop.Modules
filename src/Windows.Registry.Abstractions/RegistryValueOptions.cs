// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Windows.Registry.Abstractions;

[Flags]
public enum RegistryValueOptions
{
    None = 0x0,
    DoNotExpandEnvironmentNames = 0x1
}