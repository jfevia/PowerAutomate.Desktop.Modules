// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.Modules.ActionSelectors.Actions;

namespace PowerAutomate.Desktop.Modules.ActionSelectors.Actions.Tests;

[TestFixture]
public class SampleOptionOneActionSelectorTests
{
    [Test]
    public void Constructor_ConfiguresSelector() => Assert.That(new SampleOptionOneActionSelector(), Is.Not.Null);
}