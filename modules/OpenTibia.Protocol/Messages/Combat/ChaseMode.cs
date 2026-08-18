// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Combat;

/// <summary>
/// Whether the character chases its attack target, matching TFS's parseFightModes wire values.
/// </summary>
public enum ChaseMode : byte
{
    StandStill = 0,
    ChaseOpponent = 1
}
