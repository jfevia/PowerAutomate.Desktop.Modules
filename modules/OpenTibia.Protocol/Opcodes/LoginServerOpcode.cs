// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

/// <summary>
/// Server-to-client opcodes for the login-server connection, matching tfs-old-svn r3884.
/// </summary>
public enum LoginServerOpcode : byte
{
    None = 0x00,
    Error = 0x0A,
    Motd = 0x14,
    CharacterList = 0x64
}
