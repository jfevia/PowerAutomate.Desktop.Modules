// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.Modules.ActionSelectors.Actions;

namespace PowerAutomate.Desktop.Modules.ActionSelectors.Actions.Tests;

[TestFixture]
public class SampleOptionTwoActionSelectorTests
{
    [Test]
    public void Constructor_ConfiguresSelector() => Assert.That(new SampleOptionTwoActionSelector(), Is.Not.Null);
}