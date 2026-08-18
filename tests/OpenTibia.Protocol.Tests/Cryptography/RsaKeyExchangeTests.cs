// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Cryptography;

[TestFixture]
public class RsaKeyExchangeTests
{
    [Test]
    public void Decrypt_UndoesEncrypt()
    {
        var block = new byte[RsaKeyExchange.BlockSize];
        for (var index = 0; index < block.Length; index++)
        {
            block[index] = (byte)index;
        }

        // The leading byte stays clear so the block is numerically smaller than the modulus.
        block[0] = 0;

        var roundTripped = RsaKeyExchange.Decrypt(RsaKeyExchange.Encrypt(block));

        Assert.That(roundTripped, Is.EqualTo(block));
    }

    [Test]
    public void Encrypt_WithValueOne_IsIdentity()
    {
        var block = new byte[RsaKeyExchange.BlockSize];
        block[RsaKeyExchange.BlockSize - 1] = 1;

        var cipher = RsaKeyExchange.Encrypt(block);

        Assert.That(cipher, Is.EqualTo(block));
    }

    [Test]
    public void Encrypt_WhenResultHasHighBitSet_StillReturnsExactBlockSize()
    {
        // 2^65537 mod n has its top bit set, so BigInteger appends a sign byte that must be dropped.
        var block = new byte[RsaKeyExchange.BlockSize];
        block[RsaKeyExchange.BlockSize - 1] = 2;

        var cipher = RsaKeyExchange.Encrypt(block);

        Assert.Multiple(() =>
        {
            Assert.That(cipher, Has.Length.EqualTo(RsaKeyExchange.BlockSize));
            Assert.That(cipher[0], Is.GreaterThanOrEqualTo((byte)0x80));
            Assert.That(RsaKeyExchange.Decrypt(cipher), Is.EqualTo(block));
        });
    }

    [Test]
    public void Encrypt_WithNullBlock_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => RsaKeyExchange.Encrypt(null!));
    }

    [Test]
    public void Encrypt_WithWrongBlockSize_Throws()
    {
        Assert.Throws<ArgumentException>(() => RsaKeyExchange.Encrypt(new byte[127]));
    }

    [Test]
    public void Decrypt_WithNullBlock_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => RsaKeyExchange.Decrypt(null!));
    }

    [Test]
    public void Decrypt_WithWrongBlockSize_Throws()
    {
        Assert.Throws<ArgumentException>(() => RsaKeyExchange.Decrypt(new byte[129]));
    }
}
