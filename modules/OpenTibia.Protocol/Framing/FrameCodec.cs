// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

/// <summary>
/// Builds and parses the outer wire frame: a length prefix, an Adler-32 checksum and a body.
/// </summary>
public static class FrameCodec
{
    /// <summary>
    /// Size of the outer little-endian length prefix.
    /// </summary>
    public const int LengthPrefixSize = 2;

    /// <summary>
    /// Size of the Adler-32 checksum that follows the length prefix.
    /// </summary>
    public const int ChecksumSize = 4;

    /// <summary>
    /// Largest frame body the classic clients accept.
    /// </summary>
    public const int MaxFrameSize = 24590;

    /// <summary>
    /// Builds an unencrypted frame, used only for the very first login request.
    /// </summary>
    public static byte[] EncodePlain(byte[] payload)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return Frame(payload);
    }

    /// <summary>
    /// Builds an XTEA-encrypted frame.
    /// </summary>
    public static byte[] EncodeEncrypted(byte[] payload, uint[] key)
    {
        var sealedBody = WireCipher.Seal(payload, key);
        return Frame(sealedBody);
    }

    /// <summary>
    /// Validates the checksum of an unencrypted frame body and returns its payload.
    /// </summary>
    public static byte[] DecodePlain(byte[] body)
    {
        return Unwrap(body);
    }

    /// <summary>
    /// Builds an unencrypted server-to-client frame, which carries an inner length prefix.
    /// </summary>
    public static byte[] EncodeInboundPlain(byte[] payload)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        var writer = new PacketWriter(payload.Length + 2);
        writer.WriteUInt16((ushort)payload.Length);
        writer.WriteBytes(payload);
        return Frame(writer.ToArray());
    }

    /// <summary>
    /// Validates an unencrypted server-to-client frame and strips its inner length prefix.
    /// </summary>
    /// <remarks>
    /// Inbound frames always carry the inner length, encrypted or not; outbound first packets do not.
    /// </remarks>
    public static byte[] DecodeInboundPlain(byte[] body)
    {
        var content = Unwrap(body);
        return StripInnerLength(content);
    }

    internal static byte[] StripInnerLength(byte[] content)
    {
        if (content.Length < 2)
        {
            throw new ProtocolException("An inbound frame must contain the inner length prefix.");
        }

        var declared = content[0] | (content[1] << 8);
        if (declared + 2 > content.Length)
        {
            throw new ProtocolException(
                $"The inner length prefix declares {declared} byte(s) but only {content.Length - 2} are present.");
        }

        var payload = new byte[declared];
        Array.Copy(content, 2, payload, 0, declared);
        return payload;
    }

    /// <summary>
    /// Validates the checksum of an encrypted frame body and returns its decrypted payload.
    /// </summary>
    public static byte[] DecodeEncrypted(byte[] body, uint[] key)
    {
        var encrypted = Unwrap(body);
        return WireCipher.Open(encrypted, key);
    }

    private static byte[] Frame(byte[] body)
    {
        var size = ChecksumSize + body.Length;
        if (size > MaxFrameSize)
        {
            throw new ProtocolException($"A frame of {size} byte(s) exceeds the {MaxFrameSize} byte limit.");
        }

        var checksum = Adler32Checksum.Compute(body);

        var writer = new PacketWriter(LengthPrefixSize + size);
        writer.WriteUInt16((ushort)size);
        writer.WriteUInt32(checksum);
        writer.WriteBytes(body);
        return writer.ToArray();
    }

    private static byte[] Unwrap(byte[] body)
    {
        if (body == null)
        {
            throw new ArgumentNullException(nameof(body));
        }

        if (body.Length < ChecksumSize)
        {
            throw new ProtocolException("A frame body must contain at least the checksum.");
        }

        var expected = (uint)(body[0] | (body[1] << 8) | (body[2] << 16) | (body[3] << 24));

        var actual = Adler32Checksum.Compute(body, ChecksumSize, body.Length - ChecksumSize);
        if (actual != expected)
        {
            throw new ProtocolException(
                $"Frame checksum mismatch: expected {expected:X8} but computed {actual:X8}. The stream is desynchronized.");
        }

        var content = new byte[body.Length - ChecksumSize];
        Array.Copy(body, ChecksumSize, content, 0, content.Length);
        return content;
    }
}
