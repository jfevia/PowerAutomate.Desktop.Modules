// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Azure.Core;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.AssemblyResolution.Actions;

[Action]
public class AssemblyResolutionAction : ActionBase
{
    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var accessToken = new AccessToken();
            Debug.WriteLine($"Access token expires on {accessToken.ExpiresOn}");
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }
    }

    private static Assembly? CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        try
        {
            var assemblyName = new AssemblyName(args.Name);
            var directoryName = AppDomain.CurrentDomain.BaseDirectory;
            return !string.IsNullOrEmpty(directoryName) ? Assembly.LoadFile(Path.Combine(directoryName, $"{assemblyName.Name}.dll")) : null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}