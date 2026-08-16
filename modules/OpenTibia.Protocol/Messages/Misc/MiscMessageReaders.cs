// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Misc;

/// <summary>
/// Registers the miscellaneous payloadless notice message readers.
/// </summary>
public static class MiscMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        // The relogin window carries no fields; the opcode alone is the message.
        registry.Register(GameServerOpcode.ReLoginWindow, (opcode, reader) => new PayloadlessMessage(opcode));
    }
}
