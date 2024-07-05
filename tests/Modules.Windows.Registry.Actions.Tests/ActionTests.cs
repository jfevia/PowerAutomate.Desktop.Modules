// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System.Linq;
using System.Reflection;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Properties;

namespace PowerAutomate.Desktop.Modules.Windows.Registry.Actions.Tests;

[TestFixture]
public class ActionTests
{
    [Test]
    public void Action_All_HasLocalizableResources()
    {
        var resourceManager = Resources.ResourceManager;
        var resourcesType = typeof(Resources);
        var assembly = resourcesType.Assembly;
        var actions = assembly.ExportedTypes
                              .Select(type => (ActionType: type, ActionAttribute: type.GetCustomAttribute<ActionAttribute>()))
                              .Where(pair => pair.ActionAttribute is not null);

        foreach (var action in actions)
        {
            var friendlyNameResource = resourceManager.GetString($"{action.ActionAttribute.Id}_FriendlyName");
            var descriptionResource = resourceManager.GetString($"{action.ActionAttribute.Id}_Description");

            Assert.That(friendlyNameResource, Is.Not.Null.Or.Empty);
            Assert.That(descriptionResource, Is.Not.Null.Or.Empty);
        }
    }
}