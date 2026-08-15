// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerFx;

namespace PowerAutomate.Desktop.Modules.PowerFx.Actions;

// Pure PowerFx adapter; every member forwards straight to the engine.
[ExcludeFromCodeCoverage]
internal sealed class PowerFxEngine : IPowerFxEngine
{
    public object Evaluate(string expression) => new RecalcEngine().Eval(expression).ToObject();
}
