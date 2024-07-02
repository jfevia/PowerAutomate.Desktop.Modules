// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Win32.Internals;

internal sealed class Win32RegistryValue : Win32RegistryItem, IRegistryValue
{
    private readonly Func<object> _dataPredicate;

    internal Win32RegistryValue(string name, Func<object> dataPredicate)
        : base(name)
    {
        _dataPredicate = dataPredicate ?? throw new ArgumentNullException(nameof(dataPredicate));
    }

    public object Data => _dataPredicate();

    public override void Accept(IRegistryVisitor registryVisitor)
    {
        // Nothing
    }
}