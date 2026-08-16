// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// The account's playable character list, sent together with the message of the day.
/// </summary>
public sealed class LoginServerCharacterListMessage : IProtocolMessage
{
    public LoginServerCharacterListMessage(IReadOnlyList<CharacterListEntry> characters, ushort premiumDays)
    {
        Characters = characters ?? throw new ArgumentNullException(nameof(characters));
        PremiumDays = premiumDays;
    }

    public byte Opcode => (byte)LoginServerOpcode.CharacterList;

    public IReadOnlyList<CharacterListEntry> Characters { get; }

    public ushort PremiumDays { get; }

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteByte((byte)Characters.Count);
        foreach (var character in Characters)
        {
            character.Write(writer);
        }

        writer.WriteUInt16(PremiumDays);
    }

    public static LoginServerCharacterListMessage Read(LoginServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var count = reader.ReadByte();
        var characters = new List<CharacterListEntry>(count);
        for (var index = 0; index < count; index++)
        {
            characters.Add(CharacterListEntry.Read(reader));
        }

        var premiumDays = reader.ReadUInt16();
        return new LoginServerCharacterListMessage(characters, premiumDays);
    }
}
