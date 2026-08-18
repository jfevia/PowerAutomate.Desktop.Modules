// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Editable;

/// <summary>
/// Server-opened house rule/guest list edit window (s2c 0x97).
/// </summary>
public sealed class GameServerEditListMessage : IProtocolMessage
{
    public GameServerEditListMessage(uint windowTextId, string text)
    {
        WindowTextId = windowTextId;
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public byte Opcode => (byte)GameServerOpcode.EditList;

    public uint WindowTextId { get; }

    public string Text { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        reader.Skip(1);
        var windowTextId = reader.ReadUInt32();
        var text = reader.ReadString();
        return new GameServerEditListMessage(windowTextId, text);
    }
}
