// --------------------------------------------------------------
// Copyright (c) Power Automate for desktop. All Rights Reserved.
// --------------------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Win32.Internals;

internal abstract class Win32RegistryItem : IRegistryItem
{
    protected internal Win32RegistryItem(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public string Name { get; }

    public abstract void Accept(IRegistryVisitor registryVisitor);
}