// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.GettingStarted.Actions;

namespace PowerAutomate.Desktop.Modules.GettingStarted.Actions.Tests;

[TestFixture]
public class GettingStartedActionTests
{
    [Test]
    public void Execute_SetsGreeting()
    {
        var action = new GettingStartedAction { Name = "World" };

        action.Execute(new ActionContext());

        Assert.That(action.Message, Is.EqualTo("Hello, World!"));
    }

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new GettingStartedAction(), Is.Not.Null);
}