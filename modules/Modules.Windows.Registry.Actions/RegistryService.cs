// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions;

// Pure registry adapter; RegistryExtensions contains the exercised path logic.
[ExcludeFromCodeCoverage]
internal sealed class RegistryService : IRegistryService
{
    public IRegistryKey OpenKey(string path, bool writable) => RegistryExtensions.ParseKey(path, writable, GetHives());

    private static IEnumerable<IRegistryKey> GetHives()
    {
        yield return new RegistryKeyAdapter(global::Microsoft.Win32.Registry.ClassesRoot);
        yield return new RegistryKeyAdapter(global::Microsoft.Win32.Registry.CurrentConfig);
        yield return new RegistryKeyAdapter(global::Microsoft.Win32.Registry.CurrentUser);
        yield return new RegistryKeyAdapter(global::Microsoft.Win32.Registry.PerformanceData);
        yield return new RegistryKeyAdapter(global::Microsoft.Win32.Registry.LocalMachine);
        yield return new RegistryKeyAdapter(global::Microsoft.Win32.Registry.Users);
    }
}
