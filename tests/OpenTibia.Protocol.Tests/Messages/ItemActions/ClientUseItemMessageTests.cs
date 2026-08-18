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
public class ClientUseItemMessageTests
{
    [Test]
    public void Opcode_IsUseItem()
    {
        var message = new ClientUseItemMessage(new Position(1, 2, 3), 10, 4, 5);

        Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.UseItem));
    }

    [Test]
    public void Constructor_KeepsFields()
    {
        var message = new ClientUseItemMessage(new Position(10, 20, 7), 100, 3, 5);

        Assert.Multiple(() =>
        {
            Assert.That(message.Position, Is.EqualTo(new Position(10, 20, 7)));
            Assert.That(message.ItemId, Is.EqualTo(100));
            Assert.That(message.StackPosition, Is.EqualTo(3));
            Assert.That(message.ContainerIndex, Is.EqualTo(5));
        });
    }

    [Test]
    public void Write_EmitsPositionThenItemIdThenStackThenContainerIndex()
    {
        var writer = new PacketWriter();

        new ClientUseItemMessage(new Position(10, 20, 7), 100, 3, 5).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[]
        {
            (byte)ClientOpcode.UseItem,
            10, 0, 20, 0, 7,
            100, 0,
            3,
            5
        }));
    }
}
