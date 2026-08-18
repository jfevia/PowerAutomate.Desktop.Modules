// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PowerAutomate.Desktop.ModuleValidator;

/// <summary>
/// Loads custom modules through Power Automate Desktop's own module loader and reports what the
/// designer would see: the module name, the enums it registers and any per action errors.
/// </summary>
/// <remarks>
/// The designer only reports symptoms and reproducing a paste by hand is slow, so this runs the
/// same loader offline over every module in the repository.
/// </remarks>
public static class Program
{
    private const string DefaultPadDirectory = @"C:\Program Files (x86)\Power Automate Desktop";

    private static readonly string[] ModuleRoots = { "modules", "samples" };

    public static int Main(string[] args)
    {
        var padDirectory = ValueOf(args, "--pad") ?? DefaultPadDirectory;
        var configuration = ValueOf(args, "--configuration") ?? "Release";
        var explicitAssembly = ValueOf(args, "--assembly");

        if (!Directory.Exists(padDirectory))
        {
            Console.Error.WriteLine($"Refused: no Power Automate Desktop install at '{padDirectory}'.");
            return 2;
        }

        List<string> assemblies;
        try
        {
            assemblies = explicitAssembly != null
                ? new List<string> { Path.GetFullPath(explicitAssembly) }
                : DiscoverModuleAssemblies(configuration);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("Refused: " + exception.Message);
            return 2;
        }

        if (assemblies.Count == 0)
        {
            Console.Error.WriteLine($"Refused: no module assemblies found for configuration '{configuration}'. Build first.");
            return 2;
        }

        AppDomain.CurrentDomain.AssemblyResolve += (_, resolveArgs) => Resolve(resolveArgs.Name, assemblies, padDirectory);

        var failed = 0;
        foreach (var assembly in assemblies)
        {
            if (!Validate(assembly, padDirectory))
            {
                failed++;
            }

            Console.WriteLine();
        }

        Console.WriteLine(new string('-', 100));
        Console.WriteLine(failed == 0
            ? $"All {assemblies.Count} module(s) load cleanly."
            : $"{failed} of {assemblies.Count} module(s) reported errors.");

        return failed == 0 ? 0 : 1;
    }

    private static bool Validate(string modulePath, string padDirectory)
    {
        Console.WriteLine(new string('=', 100));
        Console.WriteLine(Path.GetFileNameWithoutExtension(modulePath));
        Console.WriteLine(new string('=', 100));

        object info;
        try
        {
            info = BuildModuleInfo(modulePath, padDirectory);
        }
        catch (Exception exception)
        {
            Console.WriteLine("LOAD FAILED: " + Flatten(exception));
            return false;
        }

        var moduleName = Get(info, "Name")?.ToString() ?? "<null>";
        Console.WriteLine($"module name : {moduleName}");

        var enums = Items(Get(info, "Enums")).ToList();
        Console.WriteLine($"enums       : {enums.Count}");
        foreach (var registered in enums)
        {
            var values = Items(Get(registered, "Values")).Select(value => value.ToString()).ToList();
            Console.WriteLine($"   {moduleName}.{Get(registered, "Name")}.<value>   [{string.Join(", ", values)}]");
        }

        var actions = Items(Get(info, "Actions")).ToList();
        var enumArguments = actions
            .SelectMany(action => Items(Get(action, "Arguments"))
                .Where(argument => Equals(Get(argument, "IsEnum"), true))
                .Select(argument => new
                {
                    Action = Get(action, "Id")?.ToString(),
                    Argument = Get(argument, "Name")?.ToString(),
                    TypeName = Get(argument, "PropertyTypeName")?.ToString(),
                    Input = Get(argument, "Input"),
                    Required = Get(argument, "Required")
                }))
            .ToList();

        if (enumArguments.Count > 0)
        {
            Console.WriteLine("enum arguments:");
            foreach (var argument in enumArguments)
            {
                var registered = enums.Any(item => string.Equals(Get(item, "Name")?.ToString(), argument.TypeName, StringComparison.OrdinalIgnoreCase));
                var verdict = registered ? "OK" : "NOT REGISTERED - the designer cannot resolve a literal for this";
                Console.WriteLine($"   {argument.Action}.{argument.Argument} : {argument.TypeName}  input={argument.Input} required={argument.Required}  {verdict}");
            }
        }

        var listOfEnum = actions
            .SelectMany(action => Items(Get(action, "Arguments"))
                .Where(argument => IsListOfEnum(Get(argument, "PropertyTypeFullName")?.ToString()))
                .Select(argument => $"{Get(action, "Id")}.{Get(argument, "Name")} : {Get(argument, "PropertyTypeName")}"))
            .ToList();

        if (listOfEnum.Count > 0)
        {
            Console.WriteLine("list of enum arguments:");
            foreach (var argument in listOfEnum)
            {
                Console.WriteLine($"   {argument}  NOT EXPRESSIBLE - a list argument is not flagged as an enum, so the "
                                  + "designer validates each element as a variable and rejects an enum literal");
            }
        }

        var failures = 0;
        foreach (var action in actions.OrderBy(action => Get(action, "Id")?.ToString(), StringComparer.Ordinal))
        {
            var errors = Items(Get(action, "Errors")).Select(error => error.ToString()).ToList();
            if (errors.Count == 0)
            {
                continue;
            }

            failures++;
            Console.WriteLine($"ACTION ERROR {Get(action, "Id")}");
            foreach (var error in errors)
            {
                Console.WriteLine("      " + error);
            }
        }

        var unregistered = enumArguments.Count(argument =>
            !enums.Any(item => string.Equals(Get(item, "Name")?.ToString(), argument.TypeName, StringComparison.OrdinalIgnoreCase)));

        var problems = failures + unregistered + listOfEnum.Count;
        Console.WriteLine(problems == 0
            ? "verdict     : OK"
            : $"verdict     : {failures} action error(s), {unregistered} unresolvable enum argument(s), {listOfEnum.Count} inexpressible list argument(s)");

        return problems == 0;
    }

    /// <summary>
    /// True for a generic list whose element type is an enum, which the designer cannot express.
    /// </summary>
    private static bool IsListOfEnum(string? propertyTypeFullName)
    {
        if (propertyTypeFullName == null || !propertyTypeFullName.StartsWith("System.Collections.Generic.List`1", StringComparison.Ordinal))
        {
            return false;
        }

        var start = propertyTypeFullName.IndexOf("[[", StringComparison.Ordinal);
        if (start < 0)
        {
            return false;
        }

        var elementName = propertyTypeFullName.Substring(start + 2).Split(',')[0];
        return Type.GetType(elementName, false)?.IsEnum
               ?? AppDomain.CurrentDomain.GetAssemblies()
                   .Select(assembly => assembly.GetType(elementName, false))
                   .FirstOrDefault(type => type != null)?.IsEnum
               ?? false;
    }

    private static object BuildModuleInfo(string modulePath, string padDirectory)
    {
        var core = Assembly.LoadFrom(Path.Combine(padDirectory, "Microsoft.Flow.RPA.Desktop.Robin.Core.dll"));
        var engine = Assembly.LoadFrom(Path.Combine(padDirectory, "Microsoft.Flow.RPA.Desktop.Robin.Engine.dll"));
        var moduleAssembly = Assembly.LoadFrom(Stage(modulePath, padDirectory));

        var typeResolver = Activator.CreateInstance(engine.GetType("Microsoft.Flow.RPA.Desktop.Robin.Engine.ModuleLoader.TypeResolver", true)!)!;
        var baseTypeInfoManager = Activator.CreateInstance(core.GetType("Microsoft.Flow.RPA.Desktop.Robin.Core.Specs.TypeInfoManagerBase", true)!)!;
        var typeInfoManagerType = core.GetType("Microsoft.Flow.RPA.Desktop.Robin.Core.Specs.TypeInfoManager", true)!;
        var typeInfoManager = Activator.CreateInstance(typeInfoManagerType, baseTypeInfoManager)!;

        var initializerType = engine.GetType("Microsoft.Flow.RPA.Desktop.Robin.Engine.ModuleLoader.ModuleBuilder.ModuleInfoInitializer", true)!;
        var initializer = Activator.CreateInstance(initializerType, typeResolver, typeInfoManager, null)!;

        var moduleType = engine.GetType("Microsoft.Flow.RPA.Desktop.Robin.Engine.ModuleLoader.Module", true)!;
        var module = Activator.CreateInstance(moduleType, Guid.NewGuid(), initializer, moduleAssembly, "ModuleValidator", true)!;

        return moduleType.GetProperty("Info")!.GetValue(module)!;
    }

    /// <summary>
    /// Copies a module and its private dependencies somewhere the loader cannot also find a second
    /// copy of an assembly Power Automate Desktop already provides, such as the actions SDK.
    /// </summary>
    /// <remarks>
    /// Two copies of the SDK mean two different ActionBase types, and the loader then rejects the
    /// module with "Assembly is not a module assembly".
    /// </remarks>
    private static string Stage(string modulePath, string padDirectory)
    {
        var source = Path.GetDirectoryName(modulePath)!;
        var staged = Path.Combine(Path.GetTempPath(), "pad-module-validator", Path.GetFileNameWithoutExtension(modulePath));
        Directory.CreateDirectory(staged);

        foreach (var file in Directory.GetFiles(source, "*.dll"))
        {
            if (File.Exists(Path.Combine(padDirectory, Path.GetFileName(file))))
            {
                continue;
            }

            File.Copy(file, Path.Combine(staged, Path.GetFileName(file)), true);
        }

        return Path.Combine(staged, Path.GetFileName(modulePath));
    }

    /// <summary>
    /// Finds every built Modules.*.Actions assembly under the repository's module roots.
    /// </summary>
    private static List<string> DiscoverModuleAssemblies(string configuration)
    {
        var root = FindRepositoryRoot();

        return ModuleRoots
            .Select(folder => Path.Combine(root, folder))
            .Where(Directory.Exists)
            .SelectMany(folder => Directory.GetDirectories(folder, "Modules.*.Actions"))
            .Select(project => Path.Combine(project, "bin", configuration, "PowerAutomate.Desktop." + Path.GetFileName(project) + ".dll"))
            .Where(File.Exists)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(Path.GetDirectoryName(typeof(Program).Assembly.Location)!);

        while (directory != null)
        {
            if (ModuleRoots.All(folder => Directory.Exists(Path.Combine(directory.FullName, folder))))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the repository root from the validator's own location.");
    }

    private static IEnumerable<object> Items(object? value)
    {
        return value is IEnumerable sequence ? sequence.Cast<object>() : Enumerable.Empty<object>();
    }

    private static object? Get(object instance, string propertyName)
    {
        return instance.GetType().GetProperty(propertyName)?.GetValue(instance);
    }

    private static string? ValueOf(IReadOnlyList<string> args, string name)
    {
        for (var index = 0; index < args.Count - 1; index++)
        {
            if (string.Equals(args[index], name, StringComparison.Ordinal))
            {
                return args[index + 1];
            }
        }

        return null;
    }

    /// <summary>
    /// Power Automate Desktop's own copies win, so the module and the loader share one ActionBase.
    /// </summary>
    private static Assembly? Resolve(string name, IEnumerable<string> modulePaths, string padDirectory)
    {
        var simpleName = new AssemblyName(name).Name + ".dll";
        var directories = new[] { padDirectory, Path.Combine(padDirectory, "dotnet") }
            .Concat(modulePaths.Select(Path.GetDirectoryName)!)
            .Distinct();

        foreach (var directory in directories)
        {
            var candidate = Path.Combine(directory!, simpleName);
            if (File.Exists(candidate))
            {
                return Assembly.LoadFrom(candidate);
            }
        }

        return null;
    }

    private static string Flatten(Exception exception)
    {
        var message = exception.Message;
        var inner = exception.InnerException;
        while (inner != null)
        {
            message += " -> " + inner.Message;
            inner = inner.InnerException;
        }

        if (exception is ReflectionTypeLoadException loadException)
        {
            message += " || " + string.Join(" | ", loadException.LoaderExceptions.Select(item => item.Message).Distinct());
        }

        return message;
    }
}
