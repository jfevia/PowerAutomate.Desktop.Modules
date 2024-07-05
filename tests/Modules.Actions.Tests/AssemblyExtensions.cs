// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Reflection;
using System.Resources;

namespace PowerAutomate.Desktop.Modules.Actions.Tests;

internal static class AssemblyExtensions
{
    public static ResourceManager GetResourceManager(this Assembly value)
    {
        var assemblyName = value.GetName();
        var resourcesType = Type.GetType($"{assemblyName.Name}.Properties.Resources", _ => value, (_, name, throwOnError) => value.GetType(name, throwOnError), true)!;
        var resourcesInstance = Activator.CreateInstance(resourcesType, true);
        var resourceManagerPropertyInfo = resourcesType.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.Static)!;
        var resourceManager = (ResourceManager)resourceManagerPropertyInfo.GetValue(resourcesInstance);
        return resourceManager;
    }
}