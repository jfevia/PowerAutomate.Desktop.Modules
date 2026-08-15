// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Categories.Actions;

namespace PowerAutomate.Desktop.Modules.Categories.Actions.Tests;

[TestFixture]
public class RootActionTests
{
    [Test]
    public void Execute_Completes() => Assert.DoesNotThrow(() => new RootAction().Execute(new ActionContext()));
}