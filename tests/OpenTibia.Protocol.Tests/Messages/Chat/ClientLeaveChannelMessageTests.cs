// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Chat;

[TestFixture]
public class ClientLeaveChannelMessageTests
{
    [Test]
    public void Opcode_IsLeaveChannel()
    {
        Assert.That(new ClientLeaveChannelMessage(1).Opcode, Is.EqualTo((byte)ClientOpcode.LeaveChannel));
    }

    [Test]
    public void Constructor_KeepsChannelId()
    {
        Assert.That(new ClientLeaveChannelMessage(7).ChannelId, Is.EqualTo(7));
    }

    [Test]
    public void Write_EmitsOpcodeThenChannelId()
    {
        var writer = new PacketWriter();

        new ClientLeaveChannelMessage(7).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { (byte)ClientOpcode.LeaveChannel, 0x07, 0x00 }));
    }
}
