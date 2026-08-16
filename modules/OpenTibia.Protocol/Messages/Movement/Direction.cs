// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;

/// <summary>
/// The eight walkable directions.
/// </summary>
public enum Direction : byte
{
    North = 0,
    East = 1,
    South = 2,
    West = 3,
    NorthEast = 4,
    SouthEast = 5,
    SouthWest = 6,
    NorthWest = 7
}

/// <summary>
/// Maps directions onto their walk and turn opcodes.
/// </summary>
public static class DirectionOpcodes
{
    public static ClientOpcode ToWalkOpcode(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return ClientOpcode.WalkNorth;
            case Direction.East:
                return ClientOpcode.WalkEast;
            case Direction.South:
                return ClientOpcode.WalkSouth;
            case Direction.West:
                return ClientOpcode.WalkWest;
            case Direction.NorthEast:
                return ClientOpcode.WalkNorthEast;
            case Direction.SouthEast:
                return ClientOpcode.WalkSouthEast;
            case Direction.SouthWest:
                return ClientOpcode.WalkSouthWest;
            case Direction.NorthWest:
                return ClientOpcode.WalkNorthWest;
            default:
                throw new ProtocolException($"Direction {direction} has no walk opcode.");
        }
    }

    /// <summary>
    /// Only the four cardinal directions can be turned toward.
    /// </summary>
    public static ClientOpcode ToTurnOpcode(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return ClientOpcode.TurnNorth;
            case Direction.East:
                return ClientOpcode.TurnEast;
            case Direction.South:
                return ClientOpcode.TurnSouth;
            case Direction.West:
                return ClientOpcode.TurnWest;
            default:
                throw new ProtocolException($"Direction {direction} cannot be turned toward.");
        }
    }

    /// <summary>
    /// Encodes a direction the way an autowalk path step expects it.
    /// </summary>
    public static byte ToPathByte(Direction direction)
    {
        switch (direction)
        {
            case Direction.East:
                return 1;
            case Direction.NorthEast:
                return 2;
            case Direction.North:
                return 3;
            case Direction.NorthWest:
                return 4;
            case Direction.West:
                return 5;
            case Direction.SouthWest:
                return 6;
            case Direction.South:
                return 7;
            case Direction.SouthEast:
                return 8;
            default:
                throw new ProtocolException($"Direction {direction} has no path encoding.");
        }
    }
}
