﻿// --------------------------------------------------------------
// Copyright (c) Power Automate for desktop. All Rights Reserved.
// --------------------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

public interface IRegistryHiveFactory
{
    IRegistryHive OpenBaseKey(RegistryHive registryHive);
    IRegistryHive OpenBaseKey(RegistryHive registryHive, RegistryView registryView);
    IRegistryHive OpenRemoteBaseKey(RegistryHive registryHive, string machineName);
    IRegistryHive OpenRemoteBaseKey(RegistryHive registryHive, string machineName, RegistryView registryView);
}