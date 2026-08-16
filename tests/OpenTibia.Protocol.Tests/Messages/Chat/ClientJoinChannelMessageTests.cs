// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Chat;

[TestFixture]
public class ClientJoinChannelMessageTests
{
    [Test]
    public void Opcode_IsJoinChannel()
    {
        Assert.That(new ClientJoinChannelMessage(1).Opcode, Is.EqualTo((byte)ClientOpcode.JoinChannel));
    }

    [Test]
    public void Constructor_KeepsChannelId()
    {
        Assert.That(new ClientJoinChannelMessage(0x1234).ChannelId, Is.EqualTo(0x1234));
    }

    [Test]
    public void Write_EmitsOpcodeThenChannelId()
    {
        var writer = new PacketWriter();

        new ClientJoinChannelMessage(0x1234).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { (byte)ClientOpcode.JoinChannel, 0x34, 0x12 }));
    }
}
