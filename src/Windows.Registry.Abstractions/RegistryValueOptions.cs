// --------------------------------------------------------------
// Copyright (c) Power Automate for desktop. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

[Flags]
public enum RegistryValueOptions
{
    None = 0x0,
    DoNotExpandEnvironmentNames = 0x1
}