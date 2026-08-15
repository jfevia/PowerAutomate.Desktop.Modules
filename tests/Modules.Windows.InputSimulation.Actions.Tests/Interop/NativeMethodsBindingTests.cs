// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Interop;

/// <summary>
/// The interop adapter is excluded from coverage because it holds no logic, which also means a
/// mistyped entry point would only surface at run time. These checks close that hole.
/// </summary>
[TestFixture]
public class NativeMethodsBindingTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _nativeMethods = typeof(IMessageDispatcher).Assembly
            .GetType("PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Interop.NativeMethods", true)!;

        _imports = _nativeMethods
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => method.Attributes.HasFlag(MethodAttributes.PinvokeImpl))
            .ToArray();
    }

    private MethodInfo[] _imports = Array.Empty<MethodInfo>();
    private Type _nativeMethods = null!;

    [Test]
    public void TheAdapterDeclaresPlatformImports()
    {
        Assert.That(_imports, Is.Not.Empty, "The interop adapter no longer declares any platform imports.");
    }

    [Test]
    public void EveryImportedEntryPointExistsInItsLibrary()
    {
        foreach (var import in _imports)
        {
            var attribute = import.GetCustomAttribute<DllImportAttribute>();
            Assert.That(attribute, Is.Not.Null, $"'{import.Name}' does not expose its import metadata.");

            var library = LoadLibrary(attribute!.Value);
            Assert.That(library, Is.Not.EqualTo(IntPtr.Zero), $"Could not load '{attribute.Value}'.");

            var entryPoint = string.IsNullOrEmpty(attribute.EntryPoint) ? import.Name : attribute.EntryPoint;
            Assert.That(GetProcAddress(library, entryPoint), Is.Not.EqualTo(IntPtr.Zero),
                $"'{attribute.Value}' does not export '{entryPoint}', declared by '{import.Name}'.");
        }
    }

    [Test]
    public void EveryImportRecordsTheLastError()
    {
        // The dispatcher decides between access denied, timeout and failure from the recorded error.
        foreach (var import in _imports)
        {
            var attribute = import.GetCustomAttribute<DllImportAttribute>()!;
            Assert.That(attribute.SetLastError, Is.True, $"'{import.Name}' does not preserve the last error.");
        }
    }

    [Test]
    public void EveryTextImportUsesTheWideEntryPoint()
    {
        foreach (var import in _imports.Where(i => i.GetCustomAttribute<DllImportAttribute>()!.CharSet == CharSet.Unicode))
        {
            var attribute = import.GetCustomAttribute<DllImportAttribute>()!;
            var entryPoint = string.IsNullOrEmpty(attribute.EntryPoint) ? import.Name : attribute.EntryPoint;

            Assert.That(entryPoint, Does.EndWith("W"), $"'{import.Name}' declares Unicode but binds '{entryPoint}'.");
        }
    }

    [Test]
    public void TheAdapterIsExcludedFromCoverage()
    {
        Assert.That(_nativeMethods.GetCustomAttribute<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute>(),
            Is.Not.Null,
            "The interop adapter must stay excluded from coverage, or it will silently lower the measured total.");
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, EntryPoint = "GetProcAddress", SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr module, string name);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW", SetLastError = true)]
    private static extern IntPtr LoadLibrary(string fileName);
}
