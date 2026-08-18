// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PowerAutomate.Desktop.Modules.Actions.Tests;

internal static class ModuleEnumerator
{
    public static IEnumerable<Assembly> GetAllAssemblies()
    {
        var type = typeof(ModuleEnumerator);
        var assembly = type.Assembly;
        var assemblyLocation = assembly.Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation)!;
        var moduleFileNames = Directory.GetFiles(assemblyDirectory, "*.Actions.dll", SearchOption.TopDirectoryOnly);
        return moduleFileNames.Select(Assembly.LoadFile).ToList();
    }

    /// <summary>
    /// Shippable modules only. Samples are deliberately minimal and skip repository conventions
    /// such as full localizable resources, but they still have to obey the engine's contract.
    /// </summary>
    public static IEnumerable<Assembly> GetProductAssemblies()
    {
        var shippable = RepositoryLayout.GetModuleAssemblyNames("modules");
        return GetAllAssemblies().Where(assembly => shippable.Contains(assembly.GetName().Name)).ToList();
    }
}