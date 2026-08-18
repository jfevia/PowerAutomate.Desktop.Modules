// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Creatures;

[TestFixture]
public class GameServerCreatureUnpassMessageTests
{
    [TestCase((byte)0, false)]
    [TestCase((byte)1, true)]
    public void Read_ParsesCreatureIdAndUnpassableFlag(byte rawFlag, bool expected)
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(666u);
        writer.WriteByte(rawFlag);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreatureUnpassMessage)GameServerCreatureUnpassMessage.Read(GameServerOpcode.CreatureUnpass, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CreatureUnpass));
            Assert.That(message.CreatureId, Is.EqualTo(666u));
            Assert.That(message.IsUnpassable, Is.EqualTo(expected));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCreatureUnpassMessage.Read(GameServerOpcode.CreatureUnpass, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3 });

        Assert.Throws<ProtocolException>(() => GameServerCreatureUnpassMessage.Read(GameServerOpcode.CreatureUnpass, reader));
    }
}
