// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;

public sealed class WindowNotFoundException : Exception
{
    public WindowNotFoundException(string criteria) : base($"Could not find a window matching {criteria}")
    {
        Criteria = criteria;
    }

    public string Criteria { get; }
}
