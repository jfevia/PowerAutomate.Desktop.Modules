// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.AssemblyResolution.Actions;

namespace PowerAutomate.Desktop.Modules.AssemblyResolution.Actions.Tests;

[TestFixture]
public class AssemblyResolutionActionTests
{
    [Test]
    public void Execute_RunsContext()
    {
        var context = new FakeAssemblyResolutionContext();
        var action = new AssemblyResolutionAction(context);

        action.Execute(new ActionContext());

        Assert.That(context.RunCount, Is.EqualTo(1));
    }

    [Test]
    public void Execute_WhenRunThrows_RemovesHandler()
    {
        var context = new FakeAssemblyResolutionContext { RunException = new InvalidOperationException("Failed") };
        var action = new AssemblyResolutionAction(context);

        Assert.Throws<InvalidOperationException>(() => action.Execute(new ActionContext()));
        Assert.DoesNotThrow(() => action.Execute(new ActionContext()));
    }


    [Test]
    public void Execute_WhenAssemblyResolveRuns_CallsResolver()
    {
        var context = new FakeAssemblyResolutionContext { ResolveDuringRun = true };
        var action = new AssemblyResolutionAction(context);

        action.Execute(new ActionContext());

        Assert.That(context.LoadedPath, Does.EndWith("Missing.Assembly.dll"));
    }

    [Test]
    public void Resolve_WithBaseDirectory_LoadsAssemblyFile()
    {
        var context = new FakeAssemblyResolutionContext { BaseDirectoryValue = "C:\\Assemblies" };
        var action = new AssemblyResolutionAction(context);

        var assembly = action.Resolve(new ResolveEventArgs("Missing.Assembly"));

        Assert.That(assembly, Is.SameAs(typeof(AssemblyResolutionActionTests).Assembly));
        Assert.That(context.LoadedPath, Is.EqualTo(Path.Combine("C:\\Assemblies", "Missing.Assembly.dll")));
    }

    [Test]
    public void Resolve_WithEmptyBaseDirectory_ReturnsNull()
    {
        var action = new AssemblyResolutionAction(new FakeAssemblyResolutionContext { BaseDirectoryValue = string.Empty });

        Assert.That(action.Resolve(new ResolveEventArgs("Missing.Assembly")), Is.Null);
    }

    [Test]
    public void Resolve_WhenLoadFails_ReturnsNull()
    {
        var action = new AssemblyResolutionAction(new FakeAssemblyResolutionContext { LoadException = new BadImageFormatException() });

        Assert.That(action.Resolve(new ResolveEventArgs("Missing.Assembly")), Is.Null);
    }

    [Test]
    public void Constructor_WithNullContext_Throws() => Assert.Throws<ArgumentNullException>(() => new AssemblyResolutionAction(null!));

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new AssemblyResolutionAction(), Is.Not.Null);

    private sealed class FakeAssemblyResolutionContext : IAssemblyResolutionContext
    {
        public string? BaseDirectoryValue { get; set; } = "C:\\Assemblies";
        public string? LoadedPath { get; private set; }
        public Exception? LoadException { get; set; }
        public Exception? RunException { get; set; }
        public bool ResolveDuringRun { get; set; }
        public int RunCount { get; private set; }
        public string? BaseDirectory => BaseDirectoryValue;

        public Assembly? LoadFile(string path)
        {
            LoadedPath = path;
            if (LoadException is not null)
            {
                throw LoadException;
            }

            return typeof(AssemblyResolutionActionTests).Assembly;
        }

        public void Run()
        {
            RunCount++;
            var exception = RunException;
            RunException = null;
            if (exception is not null)
            {
                throw exception;
            }

            if (ResolveDuringRun)
            {
                try
                {
                    Assembly.Load("Missing.Assembly");
                }
                catch (FileNotFoundException)
                {
                }
            }
        }
    }
}