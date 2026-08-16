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
public class LoginServerMotdMessageTests
{
    [Test]
    public void Write_EmitsOpcodeThenMotdString()
    {
        var writer = new PacketWriter();
        var message = new LoginServerMotdMessage("Welcome to Antica!");

        message.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteByte((byte)LoginServerOpcode.Motd);
        expectedWriter.WriteString("Welcome to Antica!");

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Read_ParsesMotdString()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Welcome to Antica!");
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = LoginServerMotdMessage.Read(LoginServerOpcode.Motd, reader);

        Assert.That(message.Motd, Is.EqualTo("Welcome to Antica!"));
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteString("Welcome to Antica!");
        var bytes = payloadWriter.ToArray();
        var reader = new PacketReader(bytes, 0, bytes.Length - 1);

        Assert.Throws<ProtocolException>(() => LoginServerMotdMessage.Read(LoginServerOpcode.Motd, reader));
    }

    [Test]
    public void Opcode_IsMotd()
    {
        Assert.That(new LoginServerMotdMessage("x").Opcode, Is.EqualTo((byte)LoginServerOpcode.Motd));
    }

    [Test]
    public void Constructor_WithNullMotd_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginServerMotdMessage(null!));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginServerMotdMessage("x").Write(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => LoginServerMotdMessage.Read(LoginServerOpcode.Motd, null!));
    }
}
