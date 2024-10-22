﻿// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.Actions.Tests;

[TestFixture]
public class ActionTests
{
    [Test]
    public void Action_All_Arguments_All_Enums_HasLocalizableResources()
    {
        var assemblies = ModuleEnumerator.GetAllAssemblies();
        foreach (var assembly in assemblies)
        {
            var resourceManager = assembly.GetResourceManager();
            var enumArgumentsByActions = assembly.ExportedTypes
                                                 .Select(type => (ActionType: type, ActionAttribute: type.GetCustomAttribute<ActionAttribute>()))
                                                 .Where(pair => pair.ActionAttribute is not null)
                                                 .Select(pair => (
                                                     pair.ActionType,
                                                     pair.ActionAttribute,
                                                     Arguments: pair.ActionType
                                                                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                    .Select(property => (Property: property, InputArgumentAttribute: property.GetCustomAttribute<InputArgumentAttribute>(), OutputArgumentAttribute: property.GetCustomAttribute<OutputArgumentAttribute>()))
                                                                    .Where(property => property.InputArgumentAttribute is not null || property.OutputArgumentAttribute is not null)
                                                                    .Where(property => property.Property.PropertyType.IsEnum)
                                                                    .ToList())
                                                 )
                                                 .Where(pair => pair.Arguments.Any())
                                                 .ToList();
            var enumValuesByPropertyTypes = enumArgumentsByActions.SelectMany(s => s.Arguments.Select(argument => argument.Property.PropertyType))
                                                                  .Distinct()
                                                                  .Select(propertyType => (PropertyType: propertyType, Values: Enum.GetValues(propertyType)
                                                                                                                                   .OfType<Enum>()
                                                                                                                                   .ToList()))
                                                                  .ToList();
            foreach (var enumValuesByPropertyType in enumValuesByPropertyTypes)
            {
                foreach (var friendlyNameResource in enumValuesByPropertyType.Values.Select(value => resourceManager.GetString($"{enumValuesByPropertyType.PropertyType.Name}_{value}_FriendlyName")))
                {
                    Assert.That(friendlyNameResource, Is.Not.Null.Or.Empty, $"Enum argument '{enumValuesByPropertyType.PropertyType.Name}' doesn't have a friendly name resource");
                }
            }
        }
    }

    [Test]
    public void Action_All_Arguments_All_HasLocalizableResources()
    {
        var assemblies = ModuleEnumerator.GetAllAssemblies();
        foreach (var assembly in assemblies)
        {
            var resourceManager = assembly.GetResourceManager();
            var argumentsByActions = assembly.ExportedTypes
                                             .Select(type => (ActionType: type, ActionAttribute: type.GetCustomAttribute<ActionAttribute>()))
                                             .Where(pair => pair.ActionAttribute is not null)
                                             .Select(pair => (
                                                 pair.ActionType,
                                                 pair.ActionAttribute,
                                                 Arguments: pair.ActionType
                                                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                .Select(property => (Property: property, InputArgumentAttribute: property.GetCustomAttribute<InputArgumentAttribute>(), OutputArgumentAttribute: property.GetCustomAttribute<OutputArgumentAttribute>()))
                                                                .Where(property => property.InputArgumentAttribute is not null || property.OutputArgumentAttribute is not null)
                                                                .ToList())
                                             )
                                             .ToList();

            foreach (var argumentsByAction in argumentsByActions)
            {
                foreach (var argument in argumentsByAction.Arguments)
                {
                    var friendlyNameResource = resourceManager.GetString($"{argumentsByAction.ActionAttribute.Id}_{argument.Property.Name}_FriendlyName");
                    var descriptionResource = resourceManager.GetString($"{argumentsByAction.ActionAttribute.Id}_{argument.Property.Name}_Description");

                    Assert.That(friendlyNameResource, Is.Not.Null.Or.Empty, $"Argument '{argument.Property.Name}' in action '{argumentsByAction.ActionAttribute.Id}' doesn't have a friendly name resource");
                    Assert.That(descriptionResource, Is.Not.Null.Or.Empty, $"Argument '{argument.Property.Name}' in action '{argumentsByAction.ActionAttribute.Id}' doesn't have a description resource");
                }
            }
        }
    }

    [Test]
    public void Action_All_Errors_All_HasLocalizableResources()
    {
        var assemblies = ModuleEnumerator.GetAllAssemblies();
        foreach (var assembly in assemblies)
        {
            var resourceManager = assembly.GetResourceManager();
            var errors = assembly.ExportedTypes
                                 .Select(type => (ActionType: type, ActionAttribute: type.GetCustomAttribute<ActionAttribute>()))
                                 .Where(pair => pair.ActionAttribute is not null)
                                 .SelectMany(pair => pair.ActionType.GetCustomAttributes<ThrowsAttribute>())
                                 .Select(attribute => attribute.Name)
                                 .Distinct()
                                 .ToList();

            foreach (var error in errors)
            {
                var friendlyNameResource = resourceManager.GetString($"Error_{error}_FriendlyName");
                var descriptionResource = resourceManager.GetString($"Error_{error}_Description");

                Assert.That(friendlyNameResource, Is.Not.Null.Or.Empty, $"Error '{error}' doesn't have a friendly name resource");
                Assert.That(descriptionResource, Is.Not.Null.Or.Empty, $"Error '{error}' doesn't have a friendly name resource");
            }
        }
    }

    [Test]
    public void Action_All_Groups_All_HasLocalizableResources()
    {
        var assemblies = ModuleEnumerator.GetAllAssemblies();
        foreach (var assembly in assemblies)
        {
            var resourceManager = assembly.GetResourceManager();
            var groups = assembly.ExportedTypes
                                 .Select(type => (ActionType: type, ActionAttribute: type.GetCustomAttribute<ActionAttribute>()))
                                 .Where(pair => pair.ActionAttribute is not null)
                                 .SelectMany(pair => pair.ActionType.GetCustomAttributes<GroupAttribute>())
                                 .Select(attribute => attribute.Name)
                                 .Distinct()
                                 .ToList();

            foreach (var group in groups)
            {
                var friendlyNameResource = resourceManager.GetString($"Group_{group}_FriendlyName");
                var descriptionResource = resourceManager.GetString($"Group_{group}_Description");

                Assert.That(friendlyNameResource, Is.Not.Null.Or.Empty, $"Group '{group}' doesn't have a friendly name resource");
                Assert.That(descriptionResource, Is.Not.Null.Or.Empty, $"Group '{group}' doesn't have a friendly name resource");
            }
        }
    }

    [Test]
    public void Action_All_HasLocalizableResources()
    {
        var assemblies = ModuleEnumerator.GetAllAssemblies();
        foreach (var assembly in assemblies)
        {
            var resourceManager = assembly.GetResourceManager();
            var actions = assembly.ExportedTypes
                                  .Select(type => (ActionType: type, ActionAttribute: type.GetCustomAttribute<ActionAttribute>()))
                                  .Where(pair => pair.ActionAttribute is not null)
                                  .ToList();

            foreach (var action in actions)
            {
                var friendlyNameResource = resourceManager.GetString($"{action.ActionAttribute.Id}_FriendlyName");
                var descriptionResource = resourceManager.GetString($"{action.ActionAttribute.Id}_Description");
                var summaryResource = resourceManager.GetString($"{action.ActionAttribute.Id}_Summary");

                Assert.That(friendlyNameResource, Is.Not.Null.Or.Empty, $"Action '{action.ActionAttribute.Id}' doesn't have a friendly name resource");
                Assert.That(descriptionResource, Is.Not.Null.Or.Empty, $"Action '{action.ActionAttribute.Id}' doesn't have a description resource");
                Assert.That(summaryResource, Is.Not.Null.Or.Empty, $"Action '{action.ActionAttribute.Id}' doesn't have a summary resource");
            }
        }
    }

    [Test]
    public void Action_Any_Exists()
    {
        var assemblies = ModuleEnumerator.GetAllAssemblies();
        foreach (var assembly in assemblies)
        {
            var assemblyTitle = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            var actions = assembly.ExportedTypes
                                  .Select(type => (ActionType: type, ActionAttribute: type.GetCustomAttribute<ActionAttribute>()))
                                  .Where(pair => pair.ActionAttribute is not null)
                                  .ToList();

            Assert.That(actions.Count, Is.Not.Zero, $"Module '{assemblyTitle.Title}' doesn't have actions");
        }
    }
}