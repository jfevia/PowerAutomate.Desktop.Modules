// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

public interface IRegistryService
{
    IRegistryKey OpenKey(string path, bool writable);
}
