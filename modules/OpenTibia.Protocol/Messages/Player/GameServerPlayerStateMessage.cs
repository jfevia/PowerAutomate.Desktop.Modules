// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Player;

/// <summary>
/// Server notice of the local player's status-icon bitmask (s2c 0xA2).
/// </summary>
public sealed class GameServerPlayerStateMessage : IProtocolMessage
{
    private const ushort PoisonedFlag = 1 << 0;
    private const ushort BurningFlag = 1 << 1;
    private const ushort EnergyFlag = 1 << 2;
    private const ushort DrunkFlag = 1 << 3;
    private const ushort ManaShieldFlag = 1 << 4;
    private const ushort ParalyzedFlag = 1 << 5;
    private const ushort HasteFlag = 1 << 6;
    private const ushort InFightFlag = 1 << 7;
    private const ushort DrowningFlag = 1 << 8;
    private const ushort FreezingFlag = 1 << 9;
    private const ushort DazzledFlag = 1 << 10;
    private const ushort CursedFlag = 1 << 11;

    public GameServerPlayerStateMessage(ushort conditions)
    {
        Conditions = conditions;
    }

    public byte Opcode => (byte)GameServerOpcode.PlayerState;

    /// <summary>
    /// The active status icons, as a raw bitmask.
    /// </summary>
    public ushort Conditions { get; }

    public bool IsPoisoned => (Conditions & PoisonedFlag) != 0;

    public bool IsBurning => (Conditions & BurningFlag) != 0;

    public bool IsEnergized => (Conditions & EnergyFlag) != 0;

    public bool IsDrunk => (Conditions & DrunkFlag) != 0;

    public bool IsManaShielded => (Conditions & ManaShieldFlag) != 0;

    public bool IsParalyzed => (Conditions & ParalyzedFlag) != 0;

    public bool IsHasted => (Conditions & HasteFlag) != 0;

    public bool IsInFight => (Conditions & InFightFlag) != 0;

    public bool IsDrowning => (Conditions & DrowningFlag) != 0;

    public bool IsFreezing => (Conditions & FreezingFlag) != 0;

    public bool IsDazzled => (Conditions & DazzledFlag) != 0;

    public bool IsCursed => (Conditions & CursedFlag) != 0;

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new GameServerPlayerStateMessage(reader.ReadUInt16());
    }
}
