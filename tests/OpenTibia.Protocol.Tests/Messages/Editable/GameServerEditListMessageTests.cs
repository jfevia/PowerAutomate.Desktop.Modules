// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Editable;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Editable;

[TestFixture]
public class GameServerEditListMessageTests
{
    [Test]
    public void Read_SkipsReservedByteThenParsesWindowIdAndText()
    {
        var writer = new PacketWriter();
        writer.WriteByte(0);
        writer.WriteUInt32(99);
        writer.WriteString("Guest\nCitizen");
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerEditListMessage)GameServerEditListMessage.Read(GameServerOpcode.EditList, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.EditList));
            Assert.That(message.WindowTextId, Is.EqualTo(99u));
            Assert.That(message.Text, Is.EqualTo("Guest\nCitizen"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullText_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerEditListMessage(0, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerEditListMessage.Read(GameServerOpcode.EditList, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 0, 1, 0 });

        Assert.Throws<ProtocolException>(() => GameServerEditListMessage.Read(GameServerOpcode.EditList, reader));
    }
}
