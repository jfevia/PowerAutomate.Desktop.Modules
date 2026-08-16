// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Numerics;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;

/// <summary>
/// Raw unpadded RSA over a single 128-byte block using the published OTServ key.
/// </summary>
/// <remarks>
/// The framework RSA providers cannot do unpadded RSA, so modular exponentiation is done directly.
/// </remarks>
public static class RsaKeyExchange
{
    /// <summary>
    /// Size in bytes of one RSA block.
    /// </summary>
    public const int BlockSize = 128;

    private const string ModulusText =
        "1091201329673994292788609605089955415282375029027981291234687579372662914925764463307396960011106039072308886100726558188253585034290575928276294364131085660290936282126359538366865626758497206207862794310902180176810615217550567108238764764442605581471797071196742839824191521181037590760306166839785666314" +
        "13";

    private const string PrivateExponentText =
        "4673033022358411862216018001503683214873298680851934467521055526294025873980576686022461064691960586020632802432670336163010988841783924195950757224728480703523556961917379229278690784579190495510360165282251912190836718788550927002538864170082173534522208794057838121087911682301377680897576685182902065907" +
        "3";

    private static readonly BigInteger Modulus = BigInteger.Parse(ModulusText);
    private static readonly BigInteger PublicExponent = new BigInteger(65537);
    private static readonly BigInteger PrivateExponent = BigInteger.Parse(PrivateExponentText);

    public static byte[] Encrypt(byte[] block)
    {
        return Transform(block, PublicExponent);
    }

    public static byte[] Decrypt(byte[] block)
    {
        return Transform(block, PrivateExponent);
    }

    private static byte[] Transform(byte[] block, BigInteger exponent)
    {
        if (block == null)
        {
            throw new ArgumentNullException(nameof(block));
        }

        if (block.Length != BlockSize)
        {
            throw new ArgumentException($"An RSA block must be exactly {BlockSize} bytes.", nameof(block));
        }

        var value = FromBigEndian(block);
        var transformed = BigInteger.ModPow(value, exponent, Modulus);
        return ToBigEndian(transformed, BlockSize);
    }

    private static BigInteger FromBigEndian(byte[] block)
    {
        // BigInteger consumes little-endian two's complement; the trailing zero forces a positive value.
        var little = new byte[block.Length + 1];
        for (var index = 0; index < block.Length; index++)
        {
            little[index] = block[block.Length - 1 - index];
        }

        return new BigInteger(little);
    }

    private static byte[] ToBigEndian(BigInteger value, int size)
    {
        var little = value.ToByteArray();
        var count = little.Length;
        if (count > size)
        {
            count = size;
        }

        var result = new byte[size];
        for (var index = 0; index < count; index++)
        {
            result[size - 1 - index] = little[index];
        }

        return result;
    }
}
