// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;

/// <summary>
/// Adler-32 checksum as used by the OpenTibia frame header.
/// </summary>
public static class Adler32Checksum
{
    private const uint Modulus = 65521;

    public static uint Compute(byte[] buffer)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        return Compute(buffer, 0, buffer.Length);
    }

    public static uint Compute(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        if (offset + count > buffer.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        uint low = 1;
        uint high = 0;

        for (var index = 0; index < count; index++)
        {
            low = (low + buffer[offset + index]) % Modulus;
            high = (high + low) % Modulus;
        }

        return (high << 16) | low;
    }
}
