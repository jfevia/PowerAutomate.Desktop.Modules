// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.OpenTibia.Protocol;

/// <summary>
/// ISO-8859-1 codec implemented inline so no encoding provider has to be registered.
/// </summary>
public static class Latin1
{
    /// <summary>
    /// Substituted for characters that do not fit in a single Latin-1 byte.
    /// </summary>
    public const byte ReplacementByte = (byte)'?';

    public static string GetString(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        var characters = new char[count];
        for (var index = 0; index < count; index++)
        {
            characters[index] = (char)buffer[offset + index];
        }

        return new string(characters);
    }

    public static byte[] GetBytes(string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var bytes = new byte[value.Length];
        for (var index = 0; index < value.Length; index++)
        {
            var character = value[index];
            bytes[index] = character > 0xFF ? ReplacementByte : (byte)character;
        }

        return bytes;
    }
}
