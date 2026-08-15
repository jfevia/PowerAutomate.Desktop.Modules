// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PowerAutomate.Desktop.Modules.AssemblyResolution.Actions;

public interface IAssemblyResolutionContext
{
    string? BaseDirectory { get; }
    Assembly? LoadFile(string path);
    void Run();
}

// Default adapter uses the current AppDomain and Assembly.LoadFile.
[ExcludeFromCodeCoverage]
internal sealed class AssemblyResolutionContext : IAssemblyResolutionContext
{
    public string? BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
    public Assembly? LoadFile(string path) => Assembly.LoadFile(path);
    public void Run() => throw new NotImplementedException();
}