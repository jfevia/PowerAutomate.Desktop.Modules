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
public class LoginServerErrorMessageTests
{
    [Test]
    public void Write_EmitsOpcodeThenErrorTextString()
    {
        var writer = new PacketWriter();
        var message = new LoginServerErrorMessage("Invalid account.");

        message.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteByte((byte)LoginServerOpcode.Error);
        expectedWriter.WriteString("Invalid account.");

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Read_ParsesErrorTextString()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Invalid account.");
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = LoginServerErrorMessage.Read(LoginServerOpcode.Error, reader);

        Assert.That(message.ErrorText, Is.EqualTo("Invalid account."));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Invalid account.");
        var bytes = payloadWriter.ToArray();
        var reader = new PacketReader(bytes, 0, bytes.Length - 1);

        Assert.Throws<ProtocolException>(() => LoginServerErrorMessage.Read(LoginServerOpcode.Error, reader));
    }

    [Test]
    public void Opcode_IsError()
    {
        Assert.That(new LoginServerErrorMessage("x").Opcode, Is.EqualTo((byte)LoginServerOpcode.Error));
    }

    [Test]
    public void Constructor_WithNullErrorText_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginServerErrorMessage(null!));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginServerErrorMessage("x").Write(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => LoginServerErrorMessage.Read(LoginServerOpcode.Error, null!));
    }
}
