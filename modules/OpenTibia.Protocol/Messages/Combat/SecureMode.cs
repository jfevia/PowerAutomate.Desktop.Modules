// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Combat;

/// <summary>
/// Whether unmarked players may be attacked, matching TFS's parseFightModes wire values.
/// </summary>
public enum SecureMode : byte
{
    CannotAttackUnmarked = 0,
    CanAttackUnmarked = 1
}
