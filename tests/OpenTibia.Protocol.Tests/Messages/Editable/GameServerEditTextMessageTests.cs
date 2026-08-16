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
public class GameServerEditTextMessageTests
{
    [Test]
    public void Read_ParsesAllFieldsInOrder()
    {
        var writer = new PacketWriter();
        writer.WriteUInt32(42);
        writer.WriteUInt16(2600);
        writer.WriteUInt16(255);
        writer.WriteString("Hello, world!");
        writer.WriteString("Gamemaster");
        writer.WriteString("Jul 12 2024 10:00:00");
        var reader = new PacketReader(writer.ToArray());

        var message = (GameServerEditTextMessage)GameServerEditTextMessage.Read(GameServerOpcode.EditText, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Opcode, Is.EqualTo((byte)GameServerOpcode.EditText));
            Assert.That(message.WindowTextId, Is.EqualTo(42u));
            Assert.That(message.ItemId, Is.EqualTo(2600));
            Assert.That(message.MaxOrCurrentLength, Is.EqualTo(255));
            Assert.That(message.Text, Is.EqualTo("Hello, world!"));
            Assert.That(message.WriterName, Is.EqualTo("Gamemaster"));
            Assert.That(message.WrittenDate, Is.EqualTo("Jul 12 2024 10:00:00"));
            Assert.That(reader.Remaining, Is.Zero);
        });
    }

    [Test]
    public void Constructor_WithNullText_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerEditTextMessage(0, 0, 0, null!, string.Empty, string.Empty));
    }

    [Test]
    public void Constructor_WithNullWriterName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerEditTextMessage(0, 0, 0, string.Empty, null!, string.Empty));
    }

    [Test]
    public void Constructor_WithNullWrittenDate_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerEditTextMessage(0, 0, 0, string.Empty, string.Empty, null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerEditTextMessage.Read(GameServerOpcode.EditText, null!));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1, 0, 0, 0 });

        Assert.Throws<ProtocolException>(() => GameServerEditTextMessage.Read(GameServerOpcode.EditText, reader));
    }
}
