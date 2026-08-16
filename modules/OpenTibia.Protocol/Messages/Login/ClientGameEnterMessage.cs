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
/// The game-server handshake: build info plus an RSA-protected XTEA key, character selection, and credentials.
/// </summary>
public sealed class ClientGameEnterMessage : IClientMessage
{
    public ClientGameEnterMessage(
        ushort os,
        ushort version,
        IReadOnlyList<uint> xteaKey,
        bool isGamemaster,
        string accountName,
        string characterName,
        string password,
        uint challengeTimestamp,
        byte challengeRandom)
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
        XteaKey = xteaKey;
        IsGamemaster = isGamemaster;
        AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
        CharacterName = characterName ?? throw new ArgumentNullException(nameof(characterName));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        ChallengeTimestamp = challengeTimestamp;
        ChallengeRandom = challengeRandom;
    }

    public byte Opcode => (byte)ClientOpcode.EnterGame;

    public ushort Os { get; }

    public ushort Version { get; }

    /// <summary>
    /// The session's 4-word XTEA key, carried inside the RSA block.
    /// </summary>
    public IReadOnlyList<uint> XteaKey { get; }

    public bool IsGamemaster { get; }

    public string AccountName { get; }

    public string CharacterName { get; }

    public string Password { get; }

    /// <summary>
    /// The timestamp echoed back from the server's <see cref="GameServerChallengeMessage"/>.
    /// </summary>
    public uint ChallengeTimestamp { get; }

    /// <summary>
    /// The random byte echoed back from the server's <see cref="GameServerChallengeMessage"/>.
    /// </summary>
    public byte ChallengeRandom { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteUInt16(Os);
        writer.WriteUInt16(Version);
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

        plaintextWriter.WriteByte((byte)(IsGamemaster ? 1 : 0));
        plaintextWriter.WriteString(AccountName);
        plaintextWriter.WriteString(CharacterName);
        plaintextWriter.WriteString(Password);
        plaintextWriter.WriteUInt32(ChallengeTimestamp);
        plaintextWriter.WriteByte(ChallengeRandom);

        var block = new byte[RsaKeyExchange.BlockSize];
        plaintextWriter.ToArray().CopyTo(block, 0);
        return RsaKeyExchange.Encrypt(block);
    }
}
