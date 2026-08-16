// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Security.Cryptography;
using PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;

namespace PowerAutomate.Desktop.OpenTibia.Client.Handshake;

/// <summary>
/// Produces the per-session XTEA key carried inside the RSA block.
/// </summary>
public static class XteaKeyGenerator
{
    public static uint[] Generate()
    {
        var bytes = new byte[XteaCipher.KeyLength * 4];

        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(bytes);
        }

        var key = new uint[XteaCipher.KeyLength];
        for (var index = 0; index < key.Length; index++)
        {
            key[index] = BitConverter.ToUInt32(bytes, index * 4);
        }

        return key;
    }
}
