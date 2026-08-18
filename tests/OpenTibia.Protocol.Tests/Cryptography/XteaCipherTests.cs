// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Cryptography;

[TestFixture]
public class XteaCipherTests
{
    private static readonly uint[] Key = { 0x00112233u, 0x44556677u, 0x8899AABBu, 0xCCDDEEFFu };

    [Test]
    public void Encrypt_WithPinnedVector_MatchesReferenceImplementation()
    {
        var cipher = XteaCipher.Encrypt(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }, Key);

        Assert.That(cipher, Is.EqualTo(new byte[] { 0xE8, 0xD9, 0xD3, 0xD9, 0xE0, 0xE5, 0x7E, 0xF1 }));
    }

    [Test]
    public void Encrypt_WithTwoBlocks_MatchesReferenceImplementation()
    {
        var plain = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        var cipher = XteaCipher.Encrypt(plain, Key);

        Assert.That(cipher, Is.EqualTo(new byte[]
        {
            0xE8, 0xD9, 0xD3, 0xD9, 0xE0, 0xE5, 0x7E, 0xF1,
            0xA0, 0xD8, 0xF8, 0xFE, 0x15, 0x67, 0x1D, 0x6B
        }));
    }

    [Test]
    public void Decrypt_UndoesEncrypt()
    {
        var plain = new byte[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 11, 12, 13, 14, 15, 16 };

        var roundTripped = XteaCipher.Decrypt(XteaCipher.Encrypt(plain, Key), Key);

        Assert.That(roundTripped, Is.EqualTo(plain));
    }

    [Test]
    public void Decrypt_WithPinnedVector_ReturnsOriginalPlaintext()
    {
        var cipher = new byte[] { 0xE8, 0xD9, 0xD3, 0xD9, 0xE0, 0xE5, 0x7E, 0xF1 };

        Assert.That(XteaCipher.Decrypt(cipher, Key), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }));
    }

    [Test]
    public void Encrypt_WithEmptyData_ReturnsEmpty()
    {
        Assert.That(XteaCipher.Encrypt(Array.Empty<byte>(), Key), Is.Empty);
    }

    [Test]
    public void Encrypt_WithNullData_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => XteaCipher.Encrypt(null!, Key));
    }

    [Test]
    public void Encrypt_WithNullKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => XteaCipher.Encrypt(new byte[8], null!));
    }

    [Test]
    public void Encrypt_WithWrongKeyLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => XteaCipher.Encrypt(new byte[8], new uint[3]));
    }

    [Test]
    public void Encrypt_WithUnalignedData_Throws()
    {
        Assert.Throws<ArgumentException>(() => XteaCipher.Encrypt(new byte[7], Key));
    }

    [Test]
    public void Decrypt_WithNullData_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => XteaCipher.Decrypt(null!, Key));
    }

    [Test]
    public void Decrypt_WithUnalignedData_Throws()
    {
        Assert.Throws<ArgumentException>(() => XteaCipher.Decrypt(new byte[3], Key));
    }
}
