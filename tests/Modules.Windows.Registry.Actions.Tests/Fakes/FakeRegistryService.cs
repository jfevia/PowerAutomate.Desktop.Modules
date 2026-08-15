// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests.Fakes;

internal sealed class FakeRegistryService : IRegistryService
{
    public FakeRegistryService(FakeRegistryKey key)
    {
        Key = key;
    }

    public Exception? ExceptionToThrow { get; set; }
    public FakeRegistryKey Key { get; }
    public List<(string Path, bool Writable)> OpenedKeys { get; } = new();

    public IRegistryKey OpenKey(string path, bool writable)
    {
        OpenedKeys.Add((path, writable));
        if (ExceptionToThrow is not null)
        {
            throw ExceptionToThrow;
        }

        return Key;
    }
}
