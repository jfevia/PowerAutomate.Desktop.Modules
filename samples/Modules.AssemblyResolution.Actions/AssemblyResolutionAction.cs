// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.AssemblyResolution.Actions;

[Action]
public class AssemblyResolutionAction : ActionBase
{
    private readonly IAssemblyResolutionContext resolutionContext;

    public AssemblyResolutionAction() : this(new AssemblyResolutionContext())
    {
    }

    public AssemblyResolutionAction(IAssemblyResolutionContext resolutionContext)
    {
        this.resolutionContext = resolutionContext ?? throw new ArgumentNullException(nameof(resolutionContext));
    }

    public override void Execute(ActionContext context)
    {
        try
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            resolutionContext.Run();
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }
    }

    public Assembly? Resolve(ResolveEventArgs args)
    {
        try
        {
            var assemblyName = new AssemblyName(args.Name);
            var directoryName = resolutionContext.BaseDirectory;
            return !string.IsNullOrEmpty(directoryName) ? resolutionContext.LoadFile(Path.Combine(directoryName, $"{assemblyName.Name}.dll")) : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private Assembly? CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) => Resolve(args);
}