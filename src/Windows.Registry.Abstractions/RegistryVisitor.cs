// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace PowerAutomate.Desktop.Windows.Registry.Abstractions;

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "PowerAutomate.Desktop")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop")]
public abstract class RegistryVisitor : IRegistryVisitor
{
    public virtual bool VisitEnterRegistry(IRegistry value)
    {
        return true;
    }

    public virtual bool VisitEnterRegistryHive(IRegistryHive value)
    {
        return true;
    }

    public virtual bool VisitEnterRegistryKey(IRegistryKey value)
    {
        return true;
    }

    public virtual bool VisitLeaveRegistry(IRegistry value)
    {
        return true;
    }

    public virtual bool VisitLeaveRegistryHive(IRegistryHive value)
    {
        return true;
    }

    public virtual bool VisitLeaveRegistryKey(IRegistryKey value)
    {
        return true;
    }

    public virtual IRegistry VisitRegistry(IRegistry value)
    {
        value.Accept(this);
        return value;
    }

    public virtual IRegistryHive VisitRegistryHive(IRegistryHive value)
    {
        value.Accept(this);
        return value;
    }

    public virtual IRegistryKey VisitRegistryKey(IRegistryKey value)
    {
        value.Accept(this);
        return value;
    }

    public virtual IRegistryKeyItem VisitRegistryKeyItem(IRegistryKeyItem value)
    {
        switch (value)
        {
            case IRegistryHive registryHive:
                VisitRegistryHive(registryHive);
                break;
            case IRegistryKey registryKey:
                VisitRegistryKey(registryKey);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
        }

        return value;
    }

    public virtual IRegistryValue VisitRegistryValue(IRegistryValue value)
    {
        value.Accept(this);
        return value;
    }
}