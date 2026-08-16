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
public class ClientUseOnCreatureMessageTests
{
    [Test]
    public void Opcode_IsUseOnCreature()
    {
        var message = new ClientUseOnCreatureMessage(new Position(1, 2, 3), 10, 4, 0x11223344);

        Assert.That(message.Opcode, Is.EqualTo((byte)ClientOpcode.UseOnCreature));
    }

    [Test]
    public void Constructor_KeepsFields()
    {
        var message = new ClientUseOnCreatureMessage(new Position(1, 2, 3), 10, 4, 0x11223344);

        Assert.Multiple(() =>
        {
            Assert.That(message.Position, Is.EqualTo(new Position(1, 2, 3)));
            Assert.That(message.ItemId, Is.EqualTo(10));
            Assert.That(message.StackPosition, Is.EqualTo(4));
            Assert.That(message.CreatureId, Is.EqualTo(0x11223344u));
        });
    }

    [Test]
    public void Write_EmitsPositionThenItemIdThenStackThenCreatureId()
    {
        var writer = new PacketWriter();

        new ClientUseOnCreatureMessage(new Position(1, 2, 3), 10, 4, 0x11223344).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new byte[]
        {
            (byte)ClientOpcode.UseOnCreature,
            1, 0, 2, 0, 3,
            10, 0,
            4,
            0x44, 0x33, 0x22, 0x11
        }));
    }
}
