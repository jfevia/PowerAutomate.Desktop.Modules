﻿// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Abstractions;

public interface IRegistryVisitable : IDisposable
{
    void Accept(IRegistryVisitor registryVisitor);
}