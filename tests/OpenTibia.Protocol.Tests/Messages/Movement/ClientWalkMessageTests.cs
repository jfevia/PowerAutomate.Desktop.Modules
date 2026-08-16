// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Movement;

[TestFixture]
public class ClientWalkMessageTests
{
    [TestCase(Direction.North, ClientOpcode.WalkNorth)]
    [TestCase(Direction.East, ClientOpcode.WalkEast)]
    [TestCase(Direction.South, ClientOpcode.WalkSouth)]
    [TestCase(Direction.West, ClientOpcode.WalkWest)]
    [TestCase(Direction.NorthEast, ClientOpcode.WalkNorthEast)]
    [TestCase(Direction.SouthEast, ClientOpcode.WalkSouthEast)]
    [TestCase(Direction.SouthWest, ClientOpcode.WalkSouthWest)]
    [TestCase(Direction.NorthWest, ClientOpcode.WalkNorthWest)]
    public void Opcode_MapsEveryDirectionToItsWalkOpcode(Direction direction, ClientOpcode expected)
    {
        Assert.That(new ClientWalkMessage(direction).Opcode, Is.EqualTo((byte)expected));
    }

    [Test]
    public void Write_EmitsOnlyTheOpcode()
    {
        var writer = new PacketWriter();

        new ClientWalkMessage(Direction.North).Write(writer);

        Assert.That(writer.ToArray(), Is.EqualTo(new[] { (byte)ClientOpcode.WalkNorth }));
    }

    [Test]
    public void Constructor_KeepsDirection()
    {
        Assert.That(new ClientWalkMessage(Direction.West).Direction, Is.EqualTo(Direction.West));
    }
}
