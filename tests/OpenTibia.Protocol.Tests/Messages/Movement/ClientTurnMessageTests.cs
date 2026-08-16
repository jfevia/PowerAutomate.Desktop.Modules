// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Movement;

[TestFixture]
public class ClientTurnMessageTests
{
    [TestCase(Direction.North, ClientOpcode.TurnNorth)]
    [TestCase(Direction.East, ClientOpcode.TurnEast)]
    [TestCase(Direction.South, ClientOpcode.TurnSouth)]
    [TestCase(Direction.West, ClientOpcode.TurnWest)]
    public void Opcode_MapsEveryCardinalDirectionToItsTurnOpcode(Direction direction, ClientOpcode expected)
    {
        Assert.That(new ClientTurnMessage(direction).Opcode, Is.EqualTo((byte)expected));
    }

    [TestCase(Direction.NorthEast)]
    [TestCase(Direction.SouthEast)]
    [TestCase(Direction.SouthWest)]
    [TestCase(Direction.NorthWest)]
    public void Opcode_WithDiagonalDirection_ThrowsProtocolException(Direction direction)
    {
        Assert.Throws<ProtocolException>(() => _ = new ClientTurnMessage(direction).Opcode);
    }

    [Test]
    public void Write_EmitsOnlyTheOpcode()
    {
        var writer = new PacketWriter();

        new ClientTurnMessage(Direction.East).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new[] { (byte)ClientOpcode.TurnEast }));
    }

    [Test]
    public void Constructor_KeepsDirection()
    {
        Assert.That(new ClientTurnMessage(Direction.South).Direction, Is.EqualTo(Direction.South));
    }
}
