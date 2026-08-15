// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.PowerFx.Actions;

public sealed class PowerFxContext
{
    public PowerFxContext(IPowerFxEngine engine)
    {
        Engine = engine ?? throw new ArgumentNullException(nameof(engine));
    }

    public IPowerFxEngine Engine { get; }

    public static PowerFxContext CreateDefault() => new(new PowerFxEngine());
}
