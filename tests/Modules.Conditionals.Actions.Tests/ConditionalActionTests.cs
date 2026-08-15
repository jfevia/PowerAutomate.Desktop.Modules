// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Conditionals.Actions;

namespace PowerAutomate.Desktop.Modules.Conditionals.Actions.Tests;

[TestFixture]
public class ConditionalActionTests
{
    [Test]
    public void Execute_PreservesResult()
    {
        var action = new ConditionalAction { Result = true };

        action.Execute(new ActionContext());

        Assert.That(action.Result, Is.True);
    }
}