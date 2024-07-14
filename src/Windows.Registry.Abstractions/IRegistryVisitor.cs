// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Windows.Registry.Abstractions;

public interface IRegistryVisitor
{
    bool VisitEnterRegistry(IRegistry value);
    bool VisitEnterRegistryHive(IRegistryHive value);
    bool VisitEnterRegistryKey(IRegistryKey value);
    bool VisitLeaveRegistry(IRegistry value);
    bool VisitLeaveRegistryHive(IRegistryHive value);
    bool VisitLeaveRegistryKey(IRegistryKey value);
    IRegistry VisitRegistry(IRegistry value);
    IRegistryHive VisitRegistryHive(IRegistryHive value);
    IRegistryKey VisitRegistryKey(IRegistryKey value);
    IRegistryKeyItem VisitRegistryKeyItem(IRegistryKeyItem value);
    IRegistryValue VisitRegistryValue(IRegistryValue value);
}