// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;

/// <summary>
/// Narrows PAD's wide numeric inputs down to the byte-sized wire types the protocol expects.
/// </summary>
public static class RangeGuard
{
    public static ushort ToUInt16(int value, string parameterName)
    {
        if (value < ushort.MinValue || value > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {ushort.MinValue} and {ushort.MaxValue}.");
        }

        return (ushort)value;
    }

    public static byte ToByte(int value, string parameterName)
    {
        if (value < byte.MinValue || value > byte.MaxValue)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {byte.MinValue} and {byte.MaxValue}.");
        }

        return (byte)value;
    }

    public static uint ToUInt32(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must not be negative.");
        }

        return (uint)value;
    }
}
