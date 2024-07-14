// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.Actions.Tests;

[TestFixture]
public class ActionSelectorTests
{
    [Test]
    public void Action_All_HasLocalizableResources()
    {
        var assemblies = ModuleEnumerator.GetAllAssemblies();
        foreach (var assembly in assemblies)
        {
            var resourceManager = assembly.GetResourceManager();
            var actionSelectorBaseType = typeof(ActionSelectorBase);
            var actionSelectorTypes = assembly.ExportedTypes
                                              .Where(type => actionSelectorBaseType.IsAssignableFrom(type))
                                              .Where(type => type is { IsGenericType: false, IsAbstract: false, IsPublic: true })
                                              .ToList();

            foreach (var actionSelectorType in actionSelectorTypes)
            {
                var instance = (ActionSelectorBase)Activator.CreateInstance(actionSelectorType);
                var summaryResource = resourceManager.GetString($"{instance.ActionName}_Summary");
                Assert.That(summaryResource, Is.Not.Null.Or.Empty, $"Action selector '{instance.ActionName}' doesn't have a summary resource");
            }
        }
    }
}