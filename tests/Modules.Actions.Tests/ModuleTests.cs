// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.Actions.Tests;

[TestFixture]
public class ModuleTests
{
    /// <summary>
    /// Guards the pitfall that started this: a module nobody referenced was silently never validated.
    /// </summary>
    [Test]
    public void Module_All_OnDisk_AreDiscovered()
    {
        var expected = RepositoryLayout.GetExpectedModuleAssemblyNames();
        var discovered = ModuleEnumerator.GetAllAssemblies()
                                         .Select(assembly => assembly.GetName().Name)
                                         .ToList();

        var missing = expected.Where(name => !discovered.Contains(name)).ToList();

        Assert.That(missing, Is.Empty,
            "These module projects exist on disk but are not loaded by the cross-module tests, so none of the "
            + "Power Automate Desktop contract rules are being enforced against them: "
            + string.Join(", ", missing));
    }

    [Test]
    public void Module_HasLocalizableResources()
    {
        var assemblies = ModuleEnumerator.GetProductAssemblies();
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