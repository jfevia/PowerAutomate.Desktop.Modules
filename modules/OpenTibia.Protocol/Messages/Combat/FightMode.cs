// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Combat;

/// <summary>
/// How the character distributes attack and defence, matching TFS's parseFightModes wire values.
/// </summary>
public enum FightMode : byte
{
    /// <summary>
    /// No fight mode; never sent on the wire.
    /// </summary>
    None = 0,
    Offensive = 1,
    Balanced = 2,
    Defensive = 3
}
