// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.ActionSelectors.Actions;

namespace PowerAutomate.Desktop.Modules.ActionSelectors.Actions.Tests;

[TestFixture]
public class SampleActionTests
{
    [TestCase(SampleOption.One, "One")]
    [TestCase(SampleOption.Two, "Two")]
    [TestCase(SampleOption.Three, "Three")]
    public void Execute_WithSampleOption_SetsSelectedGreeting(SampleOption option, string expectedName)
    {
        var action = new SampleAction { SampleOption = option, NameOne = "One", NameTwo = "Two", NameThree = "Three" };

        action.Execute(new ActionContext());

        Assert.That(action.Message, Is.EqualTo($"Hello, {expectedName}!"));
    }

    [Test]
    public void Execute_WithUnknownSampleOption_Throws()
    {
        var action = new SampleAction { SampleOption = (SampleOption)99 };

        Assert.Throws<ArgumentOutOfRangeException>(() => action.Execute(new ActionContext()));
    }

    [Test]
    public void ParameterlessConstructor_CreatesAction() => Assert.That(new SampleAction(), Is.Not.Null);
}