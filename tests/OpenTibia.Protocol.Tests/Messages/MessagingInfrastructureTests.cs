// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages;

[TestFixture]
public class PayloadlessMessageTests
{
    [Test]
    public void Write_EmitsOnlyTheOpcode()
    {
        var writer = new PacketWriter();

        new PayloadlessMessage(ClientOpcode.LeaveGame).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new[] { (byte)ClientOpcode.LeaveGame }));
    }

    [Test]
    public void Constructor_FromRawByte_KeepsOpcode()
    {
        Assert.That(new PayloadlessMessage((byte)0x42).Opcode, Is.EqualTo(0x42));
    }

    [Test]
    public void Constructor_FromGameServerOpcode_KeepsOpcode()
    {
        Assert.That(new PayloadlessMessage(GameServerOpcode.Ping).Opcode, Is.EqualTo((byte)GameServerOpcode.Ping));
    }

    [Test]
    public void ToString_RendersOpcodeInHex()
    {
        Assert.That(new PayloadlessMessage((byte)0x1E).ToString(), Is.EqualTo("opcode 0x1E"));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new PayloadlessMessage((byte)1).Write(null!));
    }
}

[TestFixture]
public class MessageWriterTests
{
    [Test]
    public void WriteOpcode_AppendsTheOpcode()
    {
        var writer = new PacketWriter();

        MessageWriter.WriteOpcode(writer, 0x7F);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { 0x7F }));
    }

    [Test]
    public void WriteOpcode_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MessageWriter.WriteOpcode(null!, 0));
    }
}

[TestFixture]
public class GameServerMessageRegistryTests
{
    private static GameServerMessageRegistry RegistryWithPing()
    {
        var registry = new GameServerMessageRegistry();
        registry.Register(GameServerOpcode.Ping, (opcode, reader) => new PayloadlessMessage(opcode));
        return registry;
    }

    [Test]
    public void ReadAll_DecodesEveryMessageInThePayload()
    {
        var registry = RegistryWithPing();

        var messages = registry.ReadAll(new[] { (byte)GameServerOpcode.Ping, (byte)GameServerOpcode.Ping });

        Assert.Multiple(() =>
        {
            Assert.That(messages, Has.Count.EqualTo(2));
            Assert.That(messages[0].Opcode, Is.EqualTo((byte)GameServerOpcode.Ping));
        });
    }

    [Test]
    public void ReadAll_WithEmptyPayload_ReturnsNothing()
    {
        Assert.That(RegistryWithPing().ReadAll(Array.Empty<byte>()), Is.Empty);
    }

    [Test]
    public void IsRegistered_ReflectsRegistration()
    {
        var registry = RegistryWithPing();

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.Ping), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.FullMap), Is.False);
        });
    }

    [Test]
    public void ReadAll_WithUnknownOpcode_ThrowsProtocolException()
    {
        var registry = RegistryWithPing();

        var exception = Assert.Throws<ProtocolException>(() => registry.ReadAll(new byte[] { 0xFE }))!;

        Assert.That(exception.Message, Does.Contain("0xFE"));
    }

    [Test]
    public void Register_WithNullReader_Throws()
    {
        var registry = new GameServerMessageRegistry();

        Assert.Throws<ArgumentNullException>(() => registry.Register(GameServerOpcode.Ping, null!));
    }

    [Test]
    public void ReadAll_WithNullPayload_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameServerMessageRegistry().ReadAll(null!));
    }
}
