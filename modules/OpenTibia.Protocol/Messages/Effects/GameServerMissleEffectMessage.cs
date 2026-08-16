// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// Server notice of a projectile flying between two positions (s2c 0x85).
/// </summary>
public sealed class GameServerMissleEffectMessage : IProtocolMessage
{
    public GameServerMissleEffectMessage(Position fromPosition, Position toPosition, ShootEffect effectType)
    {
        FromPosition = fromPosition;
        ToPosition = toPosition;
        EffectType = effectType;
    }

    public byte Opcode => (byte)GameServerOpcode.MissleEffect;

    public Position FromPosition { get; }

    public Position ToPosition { get; }

    public ShootEffect EffectType { get; }

    /// <summary>
    /// TFS writes the effect type as "actual type + 1"; this reads it back to the real type.
    /// </summary>
    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var fromPosition = PositionCodec.Read(reader);
        var toPosition = PositionCodec.Read(reader);
        var effectType = (ShootEffect)(reader.ReadByte() - 1);
        return new GameServerMissleEffectMessage(fromPosition, toPosition, effectType);
    }
}
