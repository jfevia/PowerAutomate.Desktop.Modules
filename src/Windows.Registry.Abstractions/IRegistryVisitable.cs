﻿// --------------------------------------------------------------
// Copyright (c) Power Automate for desktop. All Rights Reserved.
// --------------------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

public interface IRegistryVisitable
{
    void Accept(IRegistryVisitor registryVisitor);
}