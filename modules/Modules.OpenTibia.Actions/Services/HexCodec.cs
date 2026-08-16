// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;

/// <summary>
/// Parses the hex-string escape hatch used by the raw client message action.
/// </summary>
public static class HexCodec
{
    public static byte[] Parse(string? hex)
    {
        if (hex is null || hex.Length == 0)
        {
            return Array.Empty<byte>();
        }

        if (hex.Length % 2 != 0)
        {
            throw new ArgumentException("Hex payload must contain an even number of characters.", nameof(hex));
        }

        var bytes = new byte[hex.Length / 2];
        for (var index = 0; index < bytes.Length; index++)
        {
            bytes[index] = (byte)((NibbleValue(hex[index * 2]) << 4) | NibbleValue(hex[index * 2 + 1]));
        }

        return bytes;
    }

    private static int NibbleValue(char digit)
    {
        if (digit >= '0' && digit <= '9')
        {
            return digit - '0';
        }

        if (digit >= 'a' && digit <= 'f')
        {
            return digit - 'a' + 10;
        }

        if (digit >= 'A' && digit <= 'F')
        {
            return digit - 'A' + 10;
        }

        throw new ArgumentException($"'{digit}' is not a valid hex digit.");
    }
}
