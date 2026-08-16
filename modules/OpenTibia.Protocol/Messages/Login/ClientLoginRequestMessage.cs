// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// The login-server handshake: build info plus an RSA-protected XTEA key and account credentials.
/// </summary>
public sealed class ClientLoginRequestMessage : IClientMessage
{
    public ClientLoginRequestMessage(
        ushort os,
        ushort version,
        uint sprSignature,
        uint picSignature,
        uint datSignature,
        IReadOnlyList<uint> xteaKey,
        string accountName,
        string password)
    {
        if (xteaKey == null)
        {
            throw new ArgumentNullException(nameof(xteaKey));
        }

        if (xteaKey.Count != XteaCipher.KeyLength)
        {
            throw new ArgumentException($"An XTEA key must contain exactly {XteaCipher.KeyLength} word(s).", nameof(xteaKey));
        }

        Os = os;
        Version = version;
        SprSignature = sprSignature;
        PicSignature = picSignature;
        DatSignature = datSignature;
        XteaKey = xteaKey;
        AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
        Password = password ?? throw new ArgumentNullException(nameof(password));
    }

    public byte Opcode => (byte)ClientOpcode.LoginServerRequest;

    public ushort Os { get; }

    public ushort Version { get; }

    /// <summary>
    /// The client's <c>.spr</c> sprite-file signature.
    /// </summary>
    public uint SprSignature { get; }

    /// <summary>
    /// The client's <c>.pic</c> picture-file signature.
    /// </summary>
    public uint PicSignature { get; }

    /// <summary>
    /// The client's <c>.dat</c> data-file signature.
    /// </summary>
    public uint DatSignature { get; }

    /// <summary>
    /// The session's 4-word XTEA key, carried inside the RSA block.
    /// </summary>
    public IReadOnlyList<uint> XteaKey { get; }

    public string AccountName { get; }

    public string Password { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteUInt16(Os);
        writer.WriteUInt16(Version);
        writer.WriteUInt32(SprSignature);
        writer.WriteUInt32(PicSignature);
        writer.WriteUInt32(DatSignature);
        writer.WriteBytes(BuildRsaBlock());
    }

    private byte[] BuildRsaBlock()
    {
        var plaintextWriter = new PacketWriter();
        plaintextWriter.WriteByte(0);
        foreach (var word in XteaKey)
        {
            plaintextWriter.WriteUInt32(word);
        }

        plaintextWriter.WriteString(AccountName);
        plaintextWriter.WriteString(Password);

        var block = new byte[RsaKeyExchange.BlockSize];
        plaintextWriter.ToArray().CopyTo(block, 0);
        return RsaKeyExchange.Encrypt(block);
    }
}
