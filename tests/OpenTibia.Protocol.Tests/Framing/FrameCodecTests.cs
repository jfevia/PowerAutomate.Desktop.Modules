// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Framing;

[TestFixture]
public class FrameCodecTests
{
    private static readonly uint[] Key = { 5u, 6u, 7u, 8u };

    private static byte[] BodyOf(byte[] frame)
    {
        var body = new byte[frame.Length - FrameCodec.LengthPrefixSize];
        Array.Copy(frame, FrameCodec.LengthPrefixSize, body, 0, body.Length);
        return body;
    }

    [Test]
    public void EncodePlain_WritesLengthPrefixCoveringChecksumAndPayload()
    {
        var frame = FrameCodec.EncodePlain(new byte[] { 1, 2, 3 });

        var declared = frame[0] | (frame[1] << 8);

        Assert.Multiple(() =>
        {
            Assert.That(declared, Is.EqualTo(FrameCodec.ChecksumSize + 3));
            Assert.That(frame, Has.Length.EqualTo(FrameCodec.LengthPrefixSize + FrameCodec.ChecksumSize + 3));
        });
    }

    [Test]
    public void DecodePlain_UndoesEncodePlain()
    {
        var payload = new byte[] { 42, 43 };

        var frame = FrameCodec.EncodePlain(payload);

        Assert.That(FrameCodec.DecodePlain(BodyOf(frame)), Is.EqualTo(payload));
    }

    [Test]
    public void DecodeEncrypted_UndoesEncodeEncrypted()
    {
        var payload = new byte[] { 1, 2, 3, 4, 5 };

        var frame = FrameCodec.EncodeEncrypted(payload, Key);

        Assert.That(FrameCodec.DecodeEncrypted(BodyOf(frame), Key), Is.EqualTo(payload));
    }

    [Test]
    public void EncodeEncrypted_ProducesBlockAlignedBody()
    {
        var frame = FrameCodec.EncodeEncrypted(new byte[] { 9 }, Key);

        var encryptedLength = frame.Length - FrameCodec.LengthPrefixSize - FrameCodec.ChecksumSize;

        Assert.That(encryptedLength % XteaCipher.BlockSize, Is.Zero);
    }

    [Test]
    public void DecodePlain_WithCorruptedPayload_ThrowsProtocolException()
    {
        var frame = FrameCodec.EncodePlain(new byte[] { 1, 2, 3 });
        var body = BodyOf(frame);
        body[body.Length - 1] ^= 0xFF;

        Assert.Throws<ProtocolException>(() => FrameCodec.DecodePlain(body));
    }

    [Test]
    public void DecodeInboundPlain_StripsTheInnerLengthPrefix()
    {
        // Byte-for-byte the challenge frame a real TFS r3884 server sends.
        var frame = new byte[]
        {
            0x0C, 0x00, 0xF0, 0x00, 0xF0, 0x03, 0x06, 0x00, 0x1F, 0x87, 0x08, 0x00, 0x00, 0x3B
        };

        var payload = FrameCodec.DecodeInboundPlain(BodyOf(frame));

        Assert.That(payload, Is.EqualTo(new byte[] { 0x1F, 0x87, 0x08, 0x00, 0x00, 0x3B }));
    }

    [Test]
    public void EncodeInboundPlain_RoundTripsThroughDecodeInboundPlain()
    {
        var payload = new byte[] { 0x1F, 1, 2, 3 };

        var frame = FrameCodec.EncodeInboundPlain(payload);

        Assert.That(FrameCodec.DecodeInboundPlain(BodyOf(frame)), Is.EqualTo(payload));
    }

    [Test]
    public void EncodeInboundPlain_WithNullPayload_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => FrameCodec.EncodeInboundPlain(null!));
    }

    [Test]
    public void DecodeInboundPlain_WithoutRoomForTheInnerLength_ThrowsProtocolException()
    {
        var frame = FrameCodec.EncodePlain(new byte[] { 0x01 });

        Assert.Throws<ProtocolException>(() => FrameCodec.DecodeInboundPlain(BodyOf(frame)));
    }

    [Test]
    public void DecodeInboundPlain_WithOverstatedInnerLength_ThrowsProtocolException()
    {
        var frame = FrameCodec.EncodePlain(new byte[] { 0xFF, 0x00, 0x01 });

        Assert.Throws<ProtocolException>(() => FrameCodec.DecodeInboundPlain(BodyOf(frame)));
    }

    [Test]
    public void EncodePlain_WithNullPayload_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => FrameCodec.EncodePlain(null!));
    }

    [Test]
    public void EncodePlain_WithOversizedPayload_ThrowsProtocolException()
    {
        Assert.Throws<ProtocolException>(() => FrameCodec.EncodePlain(new byte[FrameCodec.MaxFrameSize]));
    }

    [Test]
    public void EncodePlain_AtExactSizeLimit_Succeeds()
    {
        var payload = new byte[FrameCodec.MaxFrameSize - FrameCodec.ChecksumSize];

        var frame = FrameCodec.EncodePlain(payload);

        Assert.That(frame, Has.Length.EqualTo(FrameCodec.LengthPrefixSize + FrameCodec.MaxFrameSize));
    }

    [Test]
    public void DecodePlain_WithNullBody_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => FrameCodec.DecodePlain(null!));
    }

    [Test]
    public void DecodePlain_WithBodyShorterThanChecksum_ThrowsProtocolException()
    {
        Assert.Throws<ProtocolException>(() => FrameCodec.DecodePlain(new byte[3]));
    }
}
