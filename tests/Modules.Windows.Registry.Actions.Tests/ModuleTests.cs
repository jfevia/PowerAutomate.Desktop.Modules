// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Reflection;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Properties;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests;

[TestFixture]
public class ModuleTests
{
    [Test]
    public void Module_HasLocalizableResources()
    {
        var resourceManager = Resources.ResourceManager;
        var resourcesType = typeof(Resources);
        var assembly = resourcesType.Assembly;
        var assemblyTitle = assembly.GetCustomAttribute<AssemblyTitleAttribute>();

        var friendlyNameResource = resourceManager.GetString($"{assemblyTitle.Title}_FriendlyName");
        var descriptionResource = resourceManager.GetString($"{assemblyTitle.Title}_Description");

        Assert.That(friendlyNameResource, Is.Not.Null.Or.Empty);
        Assert.That(descriptionResource, Is.Not.Null.Or.Empty);
    }
}