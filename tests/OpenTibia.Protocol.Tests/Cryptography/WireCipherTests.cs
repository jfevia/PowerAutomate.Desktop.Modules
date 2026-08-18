// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Cryptography;

[TestFixture]
public class WireCipherTests
{
    private static readonly uint[] Key = { 1u, 2u, 3u, 4u };

    [Test]
    public void Open_UndoesSeal()
    {
        var payload = new byte[] { 10, 20, 30 };

        Assert.That(WireCipher.Open(WireCipher.Seal(payload, Key), Key), Is.EqualTo(payload));
    }

    [Test]
    public void Seal_PadsToBlockBoundary()
    {
        var sealedBody = WireCipher.Seal(new byte[] { 1 }, Key);

        Assert.That(sealedBody.Length % XteaCipher.BlockSize, Is.Zero);
    }

    [Test]
    public void Seal_WithExactBlockMultiple_DoesNotAddPadding()
    {
        // Six payload bytes plus the two byte prefix already fill one block exactly.
        var sealedBody = WireCipher.Seal(new byte[6], Key);

        Assert.That(sealedBody, Has.Length.EqualTo(XteaCipher.BlockSize));
    }

    [Test]
    public void Open_WithEmptyPayload_ReturnsEmpty()
    {
        Assert.That(WireCipher.Open(WireCipher.Seal(Array.Empty<byte>(), Key), Key), Is.Empty);
    }

    [Test]
    public void Seal_WithNullPayload_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => WireCipher.Seal(null!, Key));
    }

    [Test]
    public void Open_WithNullBody_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => WireCipher.Open(null!, Key));
    }

    [Test]
    public void Open_WithEmptyBody_ThrowsProtocolException()
    {
        Assert.Throws<ProtocolException>(() => WireCipher.Open(Array.Empty<byte>(), Key));
    }

    [Test]
    public void Open_WithOverstatedInnerLength_ThrowsProtocolException()
    {
        var plain = new byte[XteaCipher.BlockSize];
        plain[0] = 0xFF;
        plain[1] = 0x00;
        var corrupted = XteaCipher.Encrypt(plain, Key);

        Assert.Throws<ProtocolException>(() => WireCipher.Open(corrupted, Key));
    }
}
