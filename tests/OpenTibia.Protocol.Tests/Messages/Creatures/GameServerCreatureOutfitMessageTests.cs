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
public class GameServerCreatureOutfitMessageTests
{
    [Test]
    public void Read_ParsesCreatureIdAndOutfit()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(333u);
        writer.WriteUInt16(128);
        writer.WriteByte(10);
        writer.WriteByte(20);
        writer.WriteByte(30);
        writer.WriteByte(40);
        writer.WriteByte(0);
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerCreatureOutfitMessage)GameServerCreatureOutfitMessage.Read(GameServerOpcode.CreatureOutfit, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.CreatureOutfit));
            Assert.That(message.CreatureId, Is.EqualTo(333u));
            Assert.That(message.Outfit.LookType, Is.EqualTo(128));
            Assert.That(message.Outfit.Head, Is.EqualTo((byte)10));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerCreatureOutfitMessage.Read(GameServerOpcode.CreatureOutfit, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 2, 3, 4 });

        Assert.Throws<ProtocolException>(() => GameServerCreatureOutfitMessage.Read(GameServerOpcode.CreatureOutfit, reader));
    }
}
