// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;

/// <summary>
/// Couples the XTEA block cipher to the length-prefixed payload the game stream carries inside it.
/// </summary>
public static class WireCipher
{
    /// <summary>
    /// Encrypts a payload as a length-prefixed, zero-padded XTEA body.
    /// </summary>
    public static byte[] Seal(byte[] payload, uint[] key)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        var declared = PacketWriterSize(payload.Length);
        var plain = new byte[declared];
        plain[0] = (byte)payload.Length;
        plain[1] = (byte)(payload.Length >> 8);
        Array.Copy(payload, 0, plain, 2, payload.Length);

        return XteaCipher.Encrypt(plain, key);
    }

    /// <summary>
    /// Decrypts an XTEA body and strips the inner length prefix.
    /// </summary>
    public static byte[] Open(byte[] encrypted, uint[] key)
    {
        if (encrypted == null)
        {
            throw new ArgumentNullException(nameof(encrypted));
        }

        var plain = XteaCipher.Decrypt(encrypted, key);
        if (plain.Length < 2)
        {
            throw new ProtocolException("An encrypted body must contain at least the inner length prefix.");
        }

        var declared = plain[0] | (plain[1] << 8);
        if (declared + 2 > plain.Length)
        {
            throw new ProtocolException(
                $"The inner length prefix declares {declared} byte(s) but only {plain.Length - 2} are present.");
        }

        var payload = new byte[declared];
        Array.Copy(plain, 2, payload, 0, declared);
        return payload;
    }

    private static int PacketWriterSize(int payloadLength)
    {
        var size = payloadLength + 2;
        var remainder = size % XteaCipher.BlockSize;
        if (remainder != 0)
        {
            size += XteaCipher.BlockSize - remainder;
        }

        return size;
    }
}
