// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PowerAutomate.Desktop.Modules.Actions.Tests;

/// <summary>
/// Finds the action module projects on disk, independently of what the test project references.
/// </summary>
internal static class RepositoryLayout
{
    private static readonly string[] ModuleRoots = { "modules", "samples" };

    /// <summary>
    /// Assembly names of every Modules.*.Actions project in the repository.
    /// </summary>
    public static IReadOnlyCollection<string> GetExpectedModuleAssemblyNames()
    {
        return ModuleRoots.SelectMany(GetModuleAssemblyNames).OrderBy(name => name).ToList();
    }

    /// <summary>
    /// Assembly names of the Modules.*.Actions projects under one root folder.
    /// </summary>
    public static IReadOnlyCollection<string> GetModuleAssemblyNames(string moduleRoot)
    {
        var folder = Path.Combine(FindRepositoryRoot(), moduleRoot);
        if (!Directory.Exists(folder))
        {
            return new List<string>();
        }

        return Directory.GetDirectories(folder, "Modules.*.Actions")
                        .Select(Path.GetFileName)
                        .Select(name => "PowerAutomate.Desktop." + name)
                        .OrderBy(name => name)
                        .ToList();
    }

    /// <summary>
    /// Walks up from the test binaries until it finds the directory holding every module root.
    /// </summary>
    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(Path.GetDirectoryName(typeof(RepositoryLayout).Assembly.Location)!);

        while (directory != null)
        {
            if (ModuleRoots.All(folder => Directory.Exists(Path.Combine(directory.FullName, folder))))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the repository root; expected an ancestor directory containing "
            + string.Join(" and ", ModuleRoots) + ".");
    }
}
