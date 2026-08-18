// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Enums;

/// <summary>
/// A walking or facing direction, mirroring the protocol values.
/// </summary>
/// <remarks>
/// Declared in the module assembly because Power Automate Desktop resolves an enum literal only
/// against enums the module itself declares.
/// </remarks>
public enum Direction : byte
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
