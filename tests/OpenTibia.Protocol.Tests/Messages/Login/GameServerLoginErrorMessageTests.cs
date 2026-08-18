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
public class GameServerLoginErrorMessageTests
{
    [Test]
    public void Write_EmitsOpcodeThenErrorTextString()
    {
        var writer = new PacketWriter();
        var message = new GameServerLoginErrorMessage("Your account is banished.");

        message.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteByte((byte)GameServerOpcode.LoginError);
        expectedWriter.WriteString("Your account is banished.");

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Read_ParsesErrorTextString()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Your account is banished.");
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = GameServerLoginErrorMessage.Read(GameServerOpcode.LoginError, reader);

        Assert.That(message.ErrorText, Is.EqualTo("Your account is banished."));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Your account is banished.");
        var bytes = payloadWriter.ToArray();
        var reader = new PacketReader(bytes, 0, bytes.Length - 1);

        Assert.Throws<ProtocolException>(() => GameServerLoginErrorMessage.Read(GameServerOpcode.LoginError, reader));
    }

    [Test]
    public void Opcode_IsLoginError()
    {
        Assert.That(new GameServerLoginErrorMessage("x").Opcode, Is.EqualTo((byte)GameServerOpcode.LoginError));
    }

    [Test]
    public void Constructor_WithNullErrorText_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerLoginErrorMessage(null!));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerLoginErrorMessage("x").Write(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => GameServerLoginErrorMessage.Read(GameServerOpcode.LoginError, null!));
    }
}
