// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Editable;

/// <summary>
/// Registers the editable text/list window message readers.
/// </summary>
public static class EditableMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(GameServerOpcode.EditText, GameServerEditTextMessage.Read);
        registry.Register(GameServerOpcode.EditList, GameServerEditListMessage.Read);
    }
}
