// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;

/// <summary>
/// 32-round XTEA over little-endian 8-byte blocks, matching the OpenTibia wire usage.
/// </summary>
public static class XteaCipher
{
    /// <summary>
    /// Every XTEA payload must be a whole number of 8-byte blocks.
    /// </summary>
    public const int BlockSize = 8;

    /// <summary>
    /// Number of 32-bit words in an XTEA key.
    /// </summary>
    public const int KeyLength = 4;

    private const uint Delta = 0x9E3779B9;
    private const int Rounds = 32;

    public static byte[] Encrypt(byte[] data, uint[] key)
    {
        Validate(data, key);

        var result = new byte[data.Length];
        Array.Copy(data, result, data.Length);

        for (var offset = 0; offset < result.Length; offset += BlockSize)
        {
            var v0 = ReadUInt32(result, offset);
            var v1 = ReadUInt32(result, offset + 4);
            uint sum = 0;

            unchecked
            {
                for (var round = 0; round < Rounds; round++)
                {
                    v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
                    sum += Delta;
                    v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
                }
            }

            WriteUInt32(result, offset, v0);
            WriteUInt32(result, offset + 4, v1);
        }

        return result;
    }

    public static byte[] Decrypt(byte[] data, uint[] key)
    {
        Validate(data, key);

        var result = new byte[data.Length];
        Array.Copy(data, result, data.Length);

        for (var offset = 0; offset < result.Length; offset += BlockSize)
        {
            var v0 = ReadUInt32(result, offset);
            var v1 = ReadUInt32(result, offset + 4);

            unchecked
            {
                var sum = Delta * Rounds;

                for (var round = 0; round < Rounds; round++)
                {
                    v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum >> 11) & 3]);
                    sum -= Delta;
                    v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
                }
            }

            WriteUInt32(result, offset, v0);
            WriteUInt32(result, offset + 4, v1);
        }

        return result;
    }

    private static void Validate(byte[] data, uint[] key)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (key.Length != KeyLength)
        {
            throw new ArgumentException($"An XTEA key must contain exactly {KeyLength} words.", nameof(key));
        }

        if (data.Length % BlockSize != 0)
        {
            throw new ArgumentException($"XTEA data must be a multiple of {BlockSize} bytes.", nameof(data));
        }
    }

    private static uint ReadUInt32(byte[] buffer, int offset)
    {
        return (uint)(buffer[offset]
                      | (buffer[offset + 1] << 8)
                      | (buffer[offset + 2] << 16)
                      | (buffer[offset + 3] << 24));
    }

    private static void WriteUInt32(byte[] buffer, int offset, uint value)
    {
        buffer[offset] = (byte)value;
        buffer[offset + 1] = (byte)(value >> 8);
        buffer[offset + 2] = (byte)(value >> 16);
        buffer[offset + 3] = (byte)(value >> 24);
    }
}
