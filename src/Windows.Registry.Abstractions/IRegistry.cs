// --------------------------------------------------------------
// Copyright (c) Power Automate for desktop. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.Generic;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

public interface IRegistry : IRegistryVisitable
{
    IRegistryHive ClassesRoot { get; }
    IRegistryHive CurrentConfig { get; }
    IRegistryHive CurrentUser { get; }
    IRegistryHive LocalMachine { get; }
    IRegistryHive PerformanceData { get; }
    IRegistryHiveFactory RegistryHive { get; }
    IRegistryHive Users { get; }
    IEnumerable<IRegistryHive> GetHives();
}