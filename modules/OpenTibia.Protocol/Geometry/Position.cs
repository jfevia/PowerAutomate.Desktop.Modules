// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;

/// <summary>
/// A map coordinate.
/// </summary>
public readonly struct Position : IEquatable<Position>
{
    public Position(ushort x, ushort y, byte z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public ushort X { get; }

    public ushort Y { get; }

    public byte Z { get; }

    public static bool operator ==(Position left, Position right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Position left, Position right)
    {
        return !left.Equals(right);
    }

    public bool Equals(Position other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
        return obj is Position other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((X * 397) ^ Y) * 397 ^ Z;
        }
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}
