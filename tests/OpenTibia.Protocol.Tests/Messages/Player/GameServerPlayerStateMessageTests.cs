// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Player;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Player;

[TestFixture]
public class GameServerPlayerStateMessageTests
{
    private static GameServerPlayerStateMessage ReadConditions(ushort conditions)
    {
        var writer = new PacketWriter();
        writer.WriteUInt16(conditions);
        var reader = new PacketReader(writer.ToArray());
        return (GameServerPlayerStateMessage)GameServerPlayerStateMessage.Read(GameServerOpcode.PlayerState, reader);
    }

    [Test]
    public void Opcode_IsPlayerState()
    {
        Assert.That(ReadConditions(0).Opcode, Is.EqualTo((byte)GameServerOpcode.PlayerState));
    }

    [Test]
    public void Read_WithZeroConditions_AllFlagsAreFalse()
    {
        var message = ReadConditions(0);

        Assert.Multiple(() =>
        {
            Assert.That(message.Conditions, Is.Zero);
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void Read_WithAllSixteenBitsSet_AllNamedFlagsAreTrueAndConditionsIsRaw()
    {
        var message = ReadConditions(0xFFFF);

        Assert.Multiple(() =>
        {
            Assert.That(message.Conditions, Is.EqualTo((ushort)0xFFFF));
            Assert.That(message.IsPoisoned, Is.True);
            Assert.That(message.IsBurning, Is.True);
            Assert.That(message.IsEnergized, Is.True);
            Assert.That(message.IsDrunk, Is.True);
            Assert.That(message.IsManaShielded, Is.True);
            Assert.That(message.IsParalyzed, Is.True);
            Assert.That(message.IsHasted, Is.True);
            Assert.That(message.IsInFight, Is.True);
            Assert.That(message.IsDrowning, Is.True);
            Assert.That(message.IsFreezing, Is.True);
            Assert.That(message.IsDazzled, Is.True);
            Assert.That(message.IsCursed, Is.True);
        });
    }

    [Test]
    public void IsPoisoned_WithOnlyPoisonBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 0);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.True);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsBurning_WithOnlyBurningBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 1);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.True);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsEnergized_WithOnlyEnergyBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 2);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.True);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsDrunk_WithOnlyDrunkBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 3);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.True);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsManaShielded_WithOnlyManaShieldBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 4);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.True);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsParalyzed_WithOnlyParalyzeBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 5);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.True);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsHasted_WithOnlyHasteBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 6);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.True);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsInFight_WithOnlyInFightBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 7);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.True);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsDrowning_WithOnlyDrowningBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 8);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.True);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsFreezing_WithOnlyFreezingBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 9);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.True);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsDazzled_WithOnlyDazzledBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 10);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.True);
            Assert.That(message.IsCursed, Is.False);
        });
    }

    [Test]
    public void IsCursed_WithOnlyCursedBitSet_IsTheOnlyTrueFlag()
    {
        var message = ReadConditions(1 << 11);

        Assert.Multiple(() =>
        {
            Assert.That(message.IsPoisoned, Is.False);
            Assert.That(message.IsBurning, Is.False);
            Assert.That(message.IsEnergized, Is.False);
            Assert.That(message.IsDrunk, Is.False);
            Assert.That(message.IsManaShielded, Is.False);
            Assert.That(message.IsParalyzed, Is.False);
            Assert.That(message.IsHasted, Is.False);
            Assert.That(message.IsInFight, Is.False);
            Assert.That(message.IsDrowning, Is.False);
            Assert.That(message.IsFreezing, Is.False);
            Assert.That(message.IsDazzled, Is.False);
            Assert.That(message.IsCursed, Is.True);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerPlayerStateMessage.Read(GameServerOpcode.PlayerState, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0x01 });

        Assert.Throws<ProtocolException>(() => GameServerPlayerStateMessage.Read(GameServerOpcode.PlayerState, reader));
    }
}
