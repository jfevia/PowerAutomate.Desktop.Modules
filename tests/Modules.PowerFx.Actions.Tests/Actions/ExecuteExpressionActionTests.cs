// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.PowerFx.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.PowerFx.Actions.Tests.Actions;

[TestFixture]
public class ExecuteExpressionActionTests
{
    [Test]
    public void Execute_EvaluatesExpressionAndStoresResult()
    {
        var engine = new FakePowerFxEngine { Result = 9 };
        var action = new ExecuteExpressionAction(new PowerFxContext(engine)) { Expression = "4 + 5" };

        action.Execute(new ActionContext());

        Assert.That(action.Result, Is.EqualTo(9));
        Assert.That(engine.LastExpression, Is.EqualTo("4 + 5"));
    }

    [Test]
    public void Execute_WhenEngineFails_ThrowsUnknownError()
    {
        var engine = new FakePowerFxEngine { ExceptionToThrow = new InvalidOperationException("boom") };
        var action = new ExecuteExpressionAction(new PowerFxContext(engine)) { Expression = "bad" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new ExecuteExpressionAction();

        Assert.That(action.Result, Is.Null);
    }
}
