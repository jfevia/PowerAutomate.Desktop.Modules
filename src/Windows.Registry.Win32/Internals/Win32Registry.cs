// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Collections.Generic;
using PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Win32.Internals;

public sealed class Win32Registry : IRegistry
{
    private IRegistryHive? _classesRoot;
    private IRegistryHive? _currentConfig;
    private IRegistryHive? _currentUser;
    private IRegistryHive? _localMachine;
    private IRegistryHive? _performanceData;
    private IRegistryHiveFactory? _registryHive;
    private IRegistryHive? _users;

    public IRegistryHive ClassesRoot => _classesRoot ??= new Win32RegistryHive(this,
        Microsoft.Win32.Registry.ClassesRoot, Microsoft.Win32.Registry.ClassesRoot.Name);

    public IRegistryHive CurrentConfig => _currentConfig ??= new Win32RegistryHive(this,
        Microsoft.Win32.Registry.CurrentConfig, Microsoft.Win32.Registry.CurrentConfig.Name);

    public IRegistryHive CurrentUser => _currentUser ??= new Win32RegistryHive(this,
        Microsoft.Win32.Registry.CurrentUser, Microsoft.Win32.Registry.CurrentUser.Name);

    public IRegistryHive LocalMachine => _localMachine ??= new Win32RegistryHive(this,
        Microsoft.Win32.Registry.LocalMachine, Microsoft.Win32.Registry.LocalMachine.Name);

    public IRegistryHive PerformanceData => _performanceData ??= new Win32RegistryHive(this,
        Microsoft.Win32.Registry.PerformanceData, Microsoft.Win32.Registry.PerformanceData.Name);

    public IRegistryHiveFactory RegistryHive => _registryHive ??= new Win32RegistryHiveFactory(this);

    public IRegistryHive Users => _users ??=
        new Win32RegistryHive(this, Microsoft.Win32.Registry.Users, Microsoft.Win32.Registry.Users.Name);

    public IEnumerable<IRegistryHive> GetHives()
    {
        yield return ClassesRoot;
        yield return CurrentUser;
        yield return LocalMachine;
        yield return Users;
        yield return CurrentConfig;
        yield return PerformanceData;
    }

    public void Accept(IRegistryVisitor registryVisitor)
    {
        if (!registryVisitor.VisitEnterRegistry(this)) return;
        foreach (var registryHive in GetHives())
        {
            if (!registryVisitor.VisitEnterRegistryHive(registryHive)) return;
            registryVisitor.VisitRegistryHive(registryHive);
            registryVisitor.VisitLeaveRegistryHive(registryHive);
        }

        registryVisitor.VisitLeaveRegistry(this);
    }
}