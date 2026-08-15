// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Services;

/// <summary>
/// One search term, used to describe what was looked for when nothing matched.
/// </summary>
public readonly struct SearchCriterion
{
    public SearchCriterion(string name, object? value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }
    public object? Value { get; }
}
