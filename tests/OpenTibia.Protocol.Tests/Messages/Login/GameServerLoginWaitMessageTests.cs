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
public class GameServerLoginWaitMessageTests
{
    [Test]
    public void Write_EmitsOpcodeTextThenWaitTime()
    {
        var writer = new PacketWriter();
        var message = new GameServerLoginWaitMessage("Queue position: 3", 20);

        message.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteByte((byte)GameServerOpcode.LoginWait);
        expectedWriter.WriteString("Queue position: 3");
        expectedWriter.WriteByte(20);

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Read_ParsesTextThenWaitTime()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Queue position: 3");
        payloadWriter.WriteByte(20);
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = GameServerLoginWaitMessage.Read(GameServerOpcode.LoginWait, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Text, Is.EqualTo("Queue position: 3"));
            Assert.That(message.WaitTimeSeconds, Is.EqualTo((byte)20));
        });
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Queue position: 3");
        payloadWriter.WriteByte(20);
        var bytes = payloadWriter.ToArray();
        var reader = new PacketReader(bytes, 0, bytes.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerLoginWaitMessage.Read(GameServerOpcode.LoginWait, reader));
    }

    [Test]
    public void Opcode_IsLoginWait()
    {
        Assert.That(new GameServerLoginWaitMessage("x", 0).Opcode, Is.EqualTo((byte)GameServerOpcode.LoginWait));
    }

    [Test]
    public void Constructor_WithNullText_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerLoginWaitMessage(null!, 0));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerLoginWaitMessage("x", 0).Write(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerLoginWaitMessage.Read(GameServerOpcode.LoginWait, null!));
    }
}
