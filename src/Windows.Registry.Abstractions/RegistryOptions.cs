// --------------------------------------------------------------
// Copyright (c) Power Automate for desktop. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

[Flags]
public enum RegistryOptions
{
    None = 0x0,
    Volatile = 0x1
}