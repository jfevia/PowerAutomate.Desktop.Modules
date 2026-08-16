// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Movement;

[TestFixture]
public class GameServerCancelWalkMessageTests
{
    [Test]
    public void Read_ParsesFacingDirection()
    {
        var reader = new PacketReader(new byte[] { (byte)Direction.SouthWest });

        var message = (GameServerCancelWalkMessage)GameServerCancelWalkMessage.Read(GameServerOpcode.CancelWalk, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CancelWalk));
            Assert.That(message.FacingDirection, Is.EqualTo(Direction.SouthWest));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCancelWalkMessage.Read(GameServerOpcode.CancelWalk, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(Array.Empty<byte>());

        Assert.Throws<ProtocolException>(() => GameServerCancelWalkMessage.Read(GameServerOpcode.CancelWalk, reader));
    }
}
