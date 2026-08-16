// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// Server notice placing a magic/graphical effect at a position (s2c 0x83).
/// </summary>
public sealed class GameServerGraphicalEffectMessage : IProtocolMessage
{
    public GameServerGraphicalEffectMessage(Position position, MagicEffect effectType)
    {
        Position = position;
        EffectType = effectType;
    }

    public byte Opcode => (byte)GameServerOpcode.GraphicalEffect;

    public Position Position { get; }

    public MagicEffect EffectType { get; }

    /// <summary>
    /// TFS writes the effect type as "actual type + 1"; this reads it back to the real type.
    /// </summary>
    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var position = PositionCodec.Read(reader);
        var effectType = (MagicEffect)(reader.ReadByte() - 1);
        return new GameServerGraphicalEffectMessage(position, effectType);
    }
}
