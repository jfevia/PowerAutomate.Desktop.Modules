// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages;

[TestFixture]
public class DirectionOpcodesTests
{
    [TestCase(Direction.North, ClientOpcode.WalkNorth)]
    [TestCase(Direction.East, ClientOpcode.WalkEast)]
    [TestCase(Direction.South, ClientOpcode.WalkSouth)]
    [TestCase(Direction.West, ClientOpcode.WalkWest)]
    [TestCase(Direction.NorthEast, ClientOpcode.WalkNorthEast)]
    [TestCase(Direction.SouthEast, ClientOpcode.WalkSouthEast)]
    [TestCase(Direction.SouthWest, ClientOpcode.WalkSouthWest)]
    [TestCase(Direction.NorthWest, ClientOpcode.WalkNorthWest)]
    public void ToWalkOpcode_MapsEveryDirection(Direction direction, ClientOpcode expected)
    {
        Assert.That(DirectionOpcodes.ToWalkOpcode(direction), Is.EqualTo(expected));
    }

    [Test]
    public void ToWalkOpcode_WithUndefinedDirection_Throws()
    {
        Assert.Throws<ProtocolException>(() => DirectionOpcodes.ToWalkOpcode((Direction)99));
    }

    [TestCase(Direction.North, ClientOpcode.TurnNorth)]
    [TestCase(Direction.East, ClientOpcode.TurnEast)]
    [TestCase(Direction.South, ClientOpcode.TurnSouth)]
    [TestCase(Direction.West, ClientOpcode.TurnWest)]
    public void ToTurnOpcode_MapsCardinalDirections(Direction direction, ClientOpcode expected)
    {
        Assert.That(DirectionOpcodes.ToTurnOpcode(direction), Is.EqualTo(expected));
    }

    [Test]
    public void ToTurnOpcode_WithDiagonalDirection_Throws()
    {
        Assert.Throws<ProtocolException>(() => DirectionOpcodes.ToTurnOpcode(Direction.NorthEast));
    }

    [TestCase(Direction.East, (byte)1)]
    [TestCase(Direction.NorthEast, (byte)2)]
    [TestCase(Direction.North, (byte)3)]
    [TestCase(Direction.NorthWest, (byte)4)]
    [TestCase(Direction.West, (byte)5)]
    [TestCase(Direction.SouthWest, (byte)6)]
    [TestCase(Direction.South, (byte)7)]
    [TestCase(Direction.SouthEast, (byte)8)]
    public void ToPathByte_MapsEveryDirection(Direction direction, byte expected)
    {
        Assert.That(DirectionOpcodes.ToPathByte(direction), Is.EqualTo(expected));
    }

    [Test]
    public void ToPathByte_WithUndefinedDirection_Throws()
    {
        Assert.Throws<ProtocolException>(() => DirectionOpcodes.ToPathByte((Direction)99));
    }
}
