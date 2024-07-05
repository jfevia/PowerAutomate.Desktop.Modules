// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.ActionSelectors;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Properties;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests;

[TestFixture]
public class ActionSelectorTests
{
    [Test]
    public void Action_All_HasLocalizableResources()
    {
        var resourceManager = Resources.ResourceManager;
        var resourcesType = typeof(Resources);
        var assembly = resourcesType.Assembly;
        var actionSelectorBaseType = typeof(ActionSelectorBase);
        var actionSelectorTypes = assembly.ExportedTypes
                                          .Where(type => actionSelectorBaseType.IsAssignableFrom(type))
                                          .Where(type => type is { IsGenericType: false, IsAbstract: false, IsPublic: true })
                                          .ToList();
        var summaryResources = actionSelectorTypes.Select(type => (ActionSelectorBase)Activator.CreateInstance(type))
                                                  .Select(instance => resourceManager.GetString($"{instance.ActionName}_Summary"));

        foreach (var summaryResource in summaryResources)
        {
            Assert.That(summaryResource, Is.Not.Null.Or.Empty);
        }
    }
}