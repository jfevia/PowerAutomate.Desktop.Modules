// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.ItemActions;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.ItemActions;

[TestFixture]
public class ClientUseItemWithMessageTests
{
    [Test]
    public void Opcode_IsUseItemWith()
    {
        var message = new ClientUseItemWithMessage(new Position(1, 2, 3), 10, 4, new Position(5, 6, 7), 20, 8);

        Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.UseItemWith));
    }

    [Test]
    public void Constructor_KeepsFields()
    {
        var message = new ClientUseItemWithMessage(new Position(1, 2, 3), 10, 4, new Position(5, 6, 7), 20, 8);

        Assert.Multiple(() =>
        {
            Assert.That(message.FromPosition, Is.EqualTo(new Position(1, 2, 3)));
            Assert.That(message.FromItemId, Is.EqualTo(10));
            Assert.That(message.FromStackPosition, Is.EqualTo(4));
            Assert.That(message.ToPosition, Is.EqualTo(new Position(5, 6, 7)));
            Assert.That(message.ToItemId, Is.EqualTo(20));
            Assert.That(message.ToStackPosition, Is.EqualTo(8));
        });
    }

    [Test]
    public void Write_EmitsFromThenTo()
    {
        var writer = new PacketWriter();

        new ClientUseItemWithMessage(new Position(1, 2, 3), 10, 4, new Position(5, 6, 7), 20, 8).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[]
        {
            (byte)ClientOpcode.UseItemWith,
            1, 0, 2, 0, 3,
            10, 0,
            4,
            5, 0, 6, 0, 7,
            20, 0,
            8
        }));
    }
}
