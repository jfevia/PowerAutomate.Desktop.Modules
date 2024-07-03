// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

namespace PowerAutomate.Desktop.Windows.Registry.Abstractions;

public interface IRegistryHiveFactory
{
    IRegistryHive OpenBaseKey(RegistryHive registryHive);
    IRegistryHive OpenBaseKey(RegistryHive registryHive, RegistryView registryView);
    IRegistryHive OpenRemoteBaseKey(RegistryHive registryHive, string machineName);
    IRegistryHive OpenRemoteBaseKey(RegistryHive registryHive, string machineName, RegistryView registryView);
}