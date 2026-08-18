// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Combat;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Combat;

[TestFixture]
public class ClientChangeFightModesMessageTests
{
    [Test]
    public void Opcode_IsChangeFightModes()
    {
        var message = new ClientChangeFightModesMessage(FightMode.Offensive, ChaseMode.StandStill, SecureMode.CannotAttackUnmarked);

        Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.ChangeFightModes));
    }

    [Test]
    public void Constructor_KeepsFields()
    {
        var message = new ClientChangeFightModesMessage(FightMode.Balanced, ChaseMode.ChaseOpponent, SecureMode.CanAttackUnmarked);

        Assert.Multiple(() =>
        {
            Assert.That(message.FightMode, Is.EqualTo(FightMode.Balanced));
            Assert.That(message.ChaseMode, Is.EqualTo(ChaseMode.ChaseOpponent));
            Assert.That(message.SecureMode, Is.EqualTo(SecureMode.CanAttackUnmarked));
        });
    }

    [TestCase(FightMode.None, ChaseMode.StandStill, SecureMode.CannotAttackUnmarked, (byte)0, (byte)0, (byte)0)]
    [TestCase(FightMode.Offensive, ChaseMode.ChaseOpponent, SecureMode.CanAttackUnmarked, (byte)1, (byte)1, (byte)1)]
    [TestCase(FightMode.Balanced, ChaseMode.StandStill, SecureMode.CanAttackUnmarked, (byte)2, (byte)0, (byte)1)]
    [TestCase(FightMode.Defensive, ChaseMode.ChaseOpponent, SecureMode.CannotAttackUnmarked, (byte)3, (byte)1, (byte)0)]
    public void Write_EmitsFightThenChaseThenSecureByte(
        FightMode fightMode, ChaseMode chaseMode, SecureMode secureMode,
        byte expectedFight, byte expectedChase, byte expectedSecure)
    {
        var writer = new PacketWriter();

        new ClientChangeFightModesMessage(fightMode, chaseMode, secureMode).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[]
        {
            (byte)ClientOpcode.ChangeFightModes, expectedFight, expectedChase, expectedSecure
        }));
    }
}
