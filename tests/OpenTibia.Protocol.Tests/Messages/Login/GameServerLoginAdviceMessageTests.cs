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
public class GameServerLoginAdviceMessageTests
{
    [Test]
    public void Write_EmitsOpcodeThenTextString()
    {
        var writer = new PacketWriter();
        var message = new GameServerLoginAdviceMessage("Your premium account expires soon.");

        message.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteByte((byte)GameServerOpcode.LoginAdvice);
        expectedWriter.WriteString("Your premium account expires soon.");

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Read_ParsesTextString()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Your premium account expires soon.");
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = GameServerLoginAdviceMessage.Read(GameServerOpcode.LoginAdvice, reader);

        Assert.That(message.Text, Is.EqualTo("Your premium account expires soon."));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Your premium account expires soon.");
        var bytes = payloadWriter.ToArray();
        var reader = new PacketReader(bytes, 0, bytes.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerLoginAdviceMessage.Read(GameServerOpcode.LoginAdvice, reader));
    }

    [Test]
    public void Opcode_IsLoginAdvice()
    {
        Assert.That(new GameServerLoginAdviceMessage("x").Opcode, Is.EqualTo((byte)GameServerOpcode.LoginAdvice));
    }

    [Test]
    public void Constructor_WithNullText_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerLoginAdviceMessage(null!));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerLoginAdviceMessage("x").Write(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerLoginAdviceMessage.Read(GameServerOpcode.LoginAdvice, null!));
    }
}
