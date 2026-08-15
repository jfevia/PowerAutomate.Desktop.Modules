// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.PowerFx.Actions.Tests.Fakes;

internal sealed class FakePowerFxEngine : IPowerFxEngine
{
    public Exception? ExceptionToThrow { get; set; }
    public string? LastExpression { get; private set; }
    public object Result { get; set; } = 42;

    public object Evaluate(string expression)
    {
        LastExpression = expression;
        if (ExceptionToThrow is not null)
        {
            throw ExceptionToThrow;
        }

        return Result;
    }
}
