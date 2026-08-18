// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Chat;

[TestFixture]
public class ClientOpenPrivateChannelMessageTests
{
    [Test]
    public void Opcode_IsOpenPrivateChannel()
    {
        Assert.That(new ClientOpenPrivateChannelMessage("Al").Opcode, Is.EqualTo((byte)ClientOpcode.OpenPrivateChannel));
    }

    [Test]
    public void Constructor_KeepsReceiverName()
    {
        Assert.That(new ClientOpenPrivateChannelMessage("Al").ReceiverName, Is.EqualTo("Al"));
    }

    [Test]
    public void Write_EmitsOpcodeThenReceiverName()
    {
        var writer = new PacketWriter();

        new ClientOpenPrivateChannelMessage("Al").Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[]
        {
            (byte)ClientOpcode.OpenPrivateChannel, 0x02, 0x00, 0x41, 0x6C
        }));
    }

    [Test]
    public void Constructor_WithNullReceiverName_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ClientOpenPrivateChannelMessage(null!));
    }
}
