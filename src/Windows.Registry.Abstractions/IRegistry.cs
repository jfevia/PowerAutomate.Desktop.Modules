// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.Generic;

namespace PowerAutomate.Desktop.Windows.Registry.Abstractions;

public interface IRegistry : IRegistryVisitable
{
    IEnumerable<IRegistryHive> GetHives();
    IRegistryHive ClassesRoot { get; }
    IRegistryHive CurrentConfig { get; }
    IRegistryHive CurrentUser { get; }
    IRegistryHive LocalMachine { get; }
    IRegistryHive PerformanceData { get; }
    IRegistryHiveFactory RegistryHive { get; }
    IRegistryHive Users { get; }
}