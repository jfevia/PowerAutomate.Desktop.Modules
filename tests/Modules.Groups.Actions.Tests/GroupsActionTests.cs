// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Categories.Actions;

namespace PowerAutomate.Desktop.Modules.Categories.Actions.Tests;

[TestFixture]
public class GroupsActionTests
{
    [Test]
    public void Execute_WithUpperCaseFalse_SetsMixedCaseGreeting()
    {
        var action = new GroupsAction { Name = "World", UpperCase = false };

        action.Execute(new ActionContext());

        Assert.That(action.Message, Is.EqualTo("Hello, World!"));
    }

    [Test]
    public void Execute_WithUpperCaseTrue_SetsUpperCaseGreeting()
    {
        var action = new GroupsAction { Name = "World", UpperCase = true };

        action.Execute(new ActionContext());

        Assert.That(action.Message, Is.EqualTo("HELLO, WORLD!"));
    }
}