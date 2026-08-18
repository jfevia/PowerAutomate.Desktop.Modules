// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
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
        var missing = new List<string>();
        var assemblies = ModuleEnumerator.GetProductAssemblies();
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
                foreach (var value in enumValuesByPropertyType.Values)
                {
                    var key = $"{enumValuesByPropertyType.PropertyType.Name}_{value}_FriendlyName";
                    if (string.IsNullOrEmpty(resourceManager.GetString(key)))
                    {
                        missing.Add($"{assembly.GetName().Name} :: {key}");
                    }
                }
            }
        }

        Assert.That(missing, Is.Empty,
            "Enum arguments need a friendly name resource per value."
            + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    [Test]
    public void Action_All_Arguments_All_HasLocalizableResources()
    {
        var assemblies = ModuleEnumerator.GetProductAssemblies();
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
        var assemblies = ModuleEnumerator.GetProductAssemblies();
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
        var assemblies = ModuleEnumerator.GetProductAssemblies();
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
        var assemblies = ModuleEnumerator.GetProductAssemblies();
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
        var assemblies = ModuleEnumerator.GetProductAssemblies();
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

    [Test]
    public void Action_All_InputArguments_All_Enums_HaveDefaultValue()
    {
        var violations = new List<string>();

        foreach (var assembly in ModuleEnumerator.GetAllAssemblies())
        {
            foreach (var (actionType, actionAttribute) in GetActions(assembly))
            {
                foreach (var property in GetInputArguments(actionType))
                {
                    if (!GetUnderlyingType(property.PropertyType).IsEnum)
                    {
                        continue;
                    }

                    if (property.GetCustomAttribute<System.ComponentModel.DefaultValueAttribute>() is null)
                    {
                        violations.Add($"{Describe(assembly, actionType, actionAttribute)} enum argument '{property.Name}' has no [DefaultValue]");
                    }
                }
            }
        }

        Assert.That(violations, Is.Empty,
            "Power Automate Desktop rejects the whole module when an enum argument has no default value."
            + Environment.NewLine + string.Join(Environment.NewLine, violations));
    }

    [Test]
    public void Action_All_Arguments_All_HaveNonReservedNames()
    {
        var assemblies = ModuleEnumerator.GetAllAssemblies();
        foreach (var assembly in assemblies)
        {
            var argumentNames = assembly.ExportedTypes
                                        .Select(type => (ActionType: type, ActionAttribute: type.GetCustomAttribute<ActionAttribute>()))
                                        .Where(pair => pair.ActionAttribute is not null)
                                        .SelectMany(pair => pair.ActionType
                                                                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                                .Where(property => property.GetCustomAttribute<InputArgumentAttribute>() is not null
                                                                                || property.GetCustomAttribute<OutputArgumentAttribute>() is not null)
                                                                .Select(property => (pair.ActionAttribute.Id, property.Name)))
                                        .ToList();

            foreach (var (actionId, propertyName) in argumentNames)
            {
                Assert.That(RobinReservedNames.Contains(propertyName), Is.False,
                    $"Argument '{propertyName}' in action '{actionId}' is a Robin reserved keyword (case-insensitive) and is rejected by the Power Automate Desktop module loader.");
            }
        }
    }

    [Test]
    public void Action_All_HaveExactlyOneConstructor()
    {
        var violations = new List<string>();

        foreach (var assembly in ModuleEnumerator.GetAllAssemblies())
        {
            foreach (var (actionType, actionAttribute) in GetActions(assembly))
            {
                var constructors = actionType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                if (constructors.Length != 1)
                {
                    violations.Add($"{Describe(assembly, actionType, actionAttribute)} declares {constructors.Length} public constructors");
                }
            }
        }

        Assert.That(violations, Is.Empty,
            "Power Automate Desktop rejects the whole module with 'Only one constructor can be defined for an "
            + "action type'. Expose test seams as settable properties instead of a second constructor."
            + Environment.NewLine + string.Join(Environment.NewLine, violations));
    }

    [Test]
    public void Action_All_InputArguments_All_NonRequired_AreNullable()
    {
        var violations = new List<string>();

        foreach (var assembly in ModuleEnumerator.GetAllAssemblies())
        {
            foreach (var (actionType, actionAttribute) in GetActions(assembly))
            {
                foreach (var property in GetInputArguments(actionType))
                {
                    var attribute = property.GetCustomAttribute<InputArgumentAttribute>()!;
                    if (attribute.Required)
                    {
                        continue;
                    }

                    var isNullable = !property.PropertyType.IsValueType
                                  || Nullable.GetUnderlyingType(property.PropertyType) is not null;
                    if (!isNullable)
                    {
                        violations.Add($"{Describe(assembly, actionType, actionAttribute)} argument '{property.Name}' is Required = false but '{property.PropertyType.FullName}' is not nullable");
                    }
                }
            }
        }

        Assert.That(violations, Is.Empty,
            "Power Automate Desktop rejects the whole module when a non-required argument is not nullable. "
            + "A [DefaultValue] does not satisfy the rule."
            + Environment.NewLine + string.Join(Environment.NewLine, violations));
    }

    private static IEnumerable<(Type ActionType, ActionAttribute ActionAttribute)> GetActions(Assembly assembly)
    {
        return assembly.ExportedTypes
                       .Select(type => (ActionType: type, ActionAttribute: type.GetCustomAttribute<ActionAttribute>()!))
                       .Where(pair => pair.ActionAttribute is not null);
    }

    private static IEnumerable<PropertyInfo> GetInputArguments(Type actionType)
    {
        return actionType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(property => property.GetCustomAttribute<InputArgumentAttribute>() is not null);
    }

    /// <summary>
    /// Names the action by module and type, because the Id is optional and often blank.
    /// </summary>
    private static string Describe(Assembly assembly, Type actionType, ActionAttribute actionAttribute)
    {
        var id = string.IsNullOrEmpty(actionAttribute.Id) ? actionType.Name : actionAttribute.Id;
        return $"{assembly.GetName().Name} :: action '{id}'";
    }

    private static Type GetUnderlyingType(Type type) => Nullable.GetUnderlyingType(type) ?? type;

    // Robin keyword tokens declared in
    // Microsoft.Flow.RPA.Desktop.Robin.Language.Parsing.LanguageLexer.
    // Robin is case-insensitive; compare accordingly.
    private static readonly System.Collections.Generic.HashSet<string> RobinReservedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Action","And","As","Block","Call","Case","Default","Disable","Else","End","Error",
        "Exit","False","For","Foreach","From","Function","Global","Goto","If","Import","In",
        "Input","Label","Loop","Mod","Next","No","Not","On","Or","Output","Repeat","Set",
        "Step","Switch","Then","Throw","Times","To","True","Wait","While","Xor","Yes"
    };
}