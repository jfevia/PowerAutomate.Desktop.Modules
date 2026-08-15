// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

/// <summary>
/// Collaborators used by registry actions.
/// </summary>
public sealed class RegistryContext
{
    public RegistryContext(IRegistryService registryService)
    {
        RegistryService = registryService ?? throw new ArgumentNullException(nameof(registryService));
    }

    public IRegistryService RegistryService { get; }

    /// <summary>
    /// Wiring used when Power Automate creates actions with Activator.
    /// </summary>
    public static RegistryContext CreateDefault() => new(new RegistryService());
}
