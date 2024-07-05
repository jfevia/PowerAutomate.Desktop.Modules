// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Reflection;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.Actions.Tests;

[TestFixture]
public class ModuleTests
{
    [Test]
    public void Module_HasLocalizableResources()
    {
        var assemblies = ModuleEnumerator.GetAllAssemblies();
        foreach (var assembly in assemblies)
        {
            var resourceManager = assembly.GetResourceManager();
            var assemblyTitle = assembly.GetCustomAttribute<AssemblyTitleAttribute>();

            var friendlyNameResource = resourceManager.GetString($"{assemblyTitle.Title}_FriendlyName");
            var descriptionResource = resourceManager.GetString($"{assemblyTitle.Title}_Description");

            Assert.That(friendlyNameResource, Is.Not.Null.Or.Empty, $"Module '{assemblyTitle.Title}' doesn't have a friendly name resource");
            Assert.That(descriptionResource, Is.Not.Null.Or.Empty, $"Module '{assemblyTitle.Title}' doesn't have a description resource");
        }
    }
}