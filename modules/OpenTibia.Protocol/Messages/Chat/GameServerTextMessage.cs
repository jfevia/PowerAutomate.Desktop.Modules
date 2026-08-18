// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Server system/status text message (s2c 0xB4).
/// </summary>
public sealed class GameServerTextMessage : IProtocolMessage
{
    public GameServerTextMessage(MessageClass messageClass, string text)
    {
        MessageClass = messageClass;
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public byte Opcode => (byte)GameServerOpcode.TextMessage;

    public MessageClass MessageClass { get; }

    public string Text { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var messageClass = (MessageClass)reader.ReadByte();
        var text = reader.ReadString();
        return new GameServerTextMessage(messageClass, text);
    }
}
