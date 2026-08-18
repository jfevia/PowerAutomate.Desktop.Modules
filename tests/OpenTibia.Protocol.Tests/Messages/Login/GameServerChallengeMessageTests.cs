// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Login;

[TestFixture]
public class GameServerChallengeMessageTests
{
    [Test]
    public void Write_EmitsOpcodeTimestampAndRandom()
    {
        var writer = new PacketWriter();
        var message = new GameServerChallengeMessage(0x12345678u, 0xAB);

        message.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteByte((byte)GameServerOpcode.Challenge);
        expectedWriter.WriteUInt32(0x12345678u);
        expectedWriter.WriteByte(0xAB);

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Read_ParsesTimestampAndRandom()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteUInt32(0x12345678u);
        payloadWriter.WriteByte(0xAB);
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = GameServerChallengeMessage.Read(GameServerOpcode.Challenge, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Timestamp, Is.EqualTo(0x12345678u));
            Assert.That(message.Random, Is.EqualTo((byte)0xAB));
        });
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteUInt32(0x12345678u);
        payloadWriter.WriteByte(0xAB);
        var bytes = payloadWriter.ToArray();
        var reader = new PacketReader(bytes, 0, bytes.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerChallengeMessage.Read(GameServerOpcode.Challenge, reader));
    }

    [Test]
    public void Opcode_IsChallenge()
    {
        Assert.That(new GameServerChallengeMessage(0, 0).Opcode, Is.EqualTo((byte)GameServerOpcode.Challenge));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerChallengeMessage(0, 0).Write(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerChallengeMessage.Read(GameServerOpcode.Challenge, null!));
    }
}
