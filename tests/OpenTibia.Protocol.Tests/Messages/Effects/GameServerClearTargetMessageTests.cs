// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Effects;

[TestFixture]
public class GameServerClearTargetMessageTests
{
    [Test]
    public void Read_ParsesTheReservedValue()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(0);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerClearTargetMessage)GameServerClearTargetMessage.Read(GameServerOpcode.ClearTarget, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.ClearTarget));
            Assert.That(message.ReservedValue, Is.Zero);
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerClearTargetMessage.Read(GameServerOpcode.ClearTarget, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0, 0 });

        Assert.Throws<ProtocolException>(() => GameServerClearTargetMessage.Read(GameServerOpcode.ClearTarget, reader));
    }
}
