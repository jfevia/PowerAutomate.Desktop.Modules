// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.PowerFx.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.PowerFx.Actions.Tests;

[TestFixture]
public class InfrastructureTests
{
    [Test]
    public void Context_RequiresEngine()
    {
        Assert.Throws<ArgumentNullException>(() => new PowerFxContext(null!));
    }

    [Test]
    public void CreateDefault_WiresEngine()
    {
        Assert.That(PowerFxContext.CreateDefault().Engine, Is.Not.Null);
    }

    [Test]
    public void Action_WithoutAContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ExecuteExpressionAction(null!));
    }

    [Test]
    public void FakeEngine_ReturnsConfiguredResult()
    {
        var engine = new FakePowerFxEngine { Result = "value" };

        Assert.That(engine.Evaluate("x"), Is.EqualTo("value"));
        Assert.That(engine.LastExpression, Is.EqualTo("x"));
    }
}
