// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Player;

/// <summary>
/// Server notice of the local player's core stats (s2c 0xA0).
/// </summary>
public sealed class GameServerPlayerDataMessage : IProtocolMessage
{
    public GameServerPlayerDataMessage(
        ushort health,
        ushort maxHealth,
        uint capacityHundredths,
        uint experience,
        ushort level,
        byte levelPercent,
        ushort mana,
        ushort maxMana,
        byte magicLevel,
        byte magicLevelPercent,
        byte soul,
        ushort staminaMinutes)
    {
        Health = health;
        MaxHealth = maxHealth;
        CapacityHundredths = capacityHundredths;
        Experience = experience;
        Level = level;
        LevelPercent = levelPercent;
        Mana = mana;
        MaxMana = maxMana;
        MagicLevel = magicLevel;
        MagicLevelPercent = magicLevelPercent;
        Soul = soul;
        StaminaMinutes = staminaMinutes;
    }

    public byte Opcode => (byte)GameServerOpcode.PlayerData;

    public ushort Health { get; }

    public ushort MaxHealth { get; }

    /// <summary>
    /// Free carrying capacity, in hundredths of an ounce.
    /// </summary>
    public uint CapacityHundredths { get; }

    /// <summary>
    /// Total experience points, capped at 0x7FFFFFFF.
    /// </summary>
    public uint Experience { get; }

    public ushort Level { get; }

    public byte LevelPercent { get; }

    public ushort Mana { get; }

    public ushort MaxMana { get; }

    public byte MagicLevel { get; }

    public byte MagicLevelPercent { get; }

    public byte Soul { get; }

    /// <summary>
    /// Remaining stamina, in minutes.
    /// </summary>
    public ushort StaminaMinutes { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var health = reader.ReadUInt16();
        var maxHealth = reader.ReadUInt16();
        var capacityHundredths = reader.ReadUInt32();
        var experience = reader.ReadUInt32();
        var level = reader.ReadUInt16();
        var levelPercent = reader.ReadByte();
        var mana = reader.ReadUInt16();
        var maxMana = reader.ReadUInt16();
        var magicLevel = reader.ReadByte();
        var magicLevelPercent = reader.ReadByte();
        var soul = reader.ReadByte();
        var staminaMinutes = reader.ReadUInt16();
        return new GameServerPlayerDataMessage(
            health, maxHealth, capacityHundredths, experience, level, levelPercent,
            mana, maxMana, magicLevel, magicLevelPercent, soul, staminaMinutes);
    }
}
