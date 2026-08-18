// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Enums;

/// <summary>
/// A walking or facing direction, mirroring the protocol values.
/// </summary>
/// <remarks>
/// Declared in the module assembly, and backed by int, because the module loader reads a value with
/// (int)Enum.Parse and silently drops the enum when the underlying type is anything narrower.
/// </remarks>
public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3,
    NorthEast = 4,
    SouthEast = 5,
    SouthWest = 6,
    NorthWest = 7
}
