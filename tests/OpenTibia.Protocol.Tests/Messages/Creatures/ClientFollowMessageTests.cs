// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Creatures;

[TestFixture]
public class ClientFollowMessageTests
{
    [Test]
    public void Opcode_IsFollow()
    {
        Assert.That(new ClientFollowMessage(1, 2).Opcode, Is.EqualTo((byte)ClientOpcode.Follow));
    }

    [Test]
    public void Constructor_KeepsCreatureIdAndSequenceNumber()
    {
        var message = new ClientFollowMessage(0x12345678, 0x9ABCDEF0);

        Assert.Multiple(() =>
        {
            Assert.That(message.CreatureId, Is.EqualTo(0x12345678u));
            Assert.That(message.SequenceNumber, Is.EqualTo(0x9ABCDEF0u));
        });
    }

    [Test]
    public void Write_EmitsOpcodeThenCreatureIdThenSequenceNumber()
    {
        var writer = new PacketWriter();

        new ClientFollowMessage(0x12345678, 0x9ABCDEF0).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[]
        {
            (byte)ClientOpcode.Follow,
            0x78, 0x56, 0x34, 0x12,
            0xF0, 0xDE, 0xBC, 0x9A
        }));
    }

    [Test]
    public void Write_WithZeroCreatureId_CancelsCurrentFollow()
    {
        var writer = new PacketWriter();

        new ClientFollowMessage(0, 0).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[] { (byte)ClientOpcode.Follow, 0, 0, 0, 0, 0, 0, 0, 0 }));
    }
}
