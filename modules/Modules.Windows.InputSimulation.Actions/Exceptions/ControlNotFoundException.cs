// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;

public sealed class ControlNotFoundException : Exception
{
    public ControlNotFoundException(string criteria) : base($"Could not find a control matching {criteria}")
    {
        Criteria = criteria;
    }

    public string Criteria { get; }
}
