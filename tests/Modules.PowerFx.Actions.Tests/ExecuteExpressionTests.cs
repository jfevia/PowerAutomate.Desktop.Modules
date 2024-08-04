// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.PowerFx.Actions.Tests;

[TestFixture]
public class ExecuteExpressionTests
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
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

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
    }

    [Test]
    public void Execute()
    {
        var action = new ExecuteExpressionAction
        {
            Expression = "1+2+3"
        };

        Assert.DoesNotThrow(() => action.Execute(new ActionContext()));
    }
}