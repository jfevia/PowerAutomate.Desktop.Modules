// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// Server notice placing floating combat/status text at a position (s2c 0x84).
/// </summary>
public sealed class GameServerTextEffectMessage : IProtocolMessage
{
    public GameServerTextEffectMessage(Position position, TextColor color, string text)
    {
        Position = position;
        Color = color;
        Text = text;
    }

    public byte Opcode => (byte)GameServerOpcode.TextEffect;

    public Position Position { get; }

    public TextColor Color { get; }

    public string Text { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var position = PositionCodec.Read(reader);
        var color = (TextColor)reader.ReadByte();
        var text = reader.ReadString();
        return new GameServerTextEffectMessage(position, color, text);
    }
}
