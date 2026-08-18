// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Login;

[TestFixture]
public class LoginServerCharacterListMessageTests
{
    private static CharacterListEntry MakeEntry(string name, string world, uint address, ushort port)
    {
        return new CharacterListEntry(name, world, address, port);
    }

    [Test]
    public void Write_WithNoCharacters_EmitsZeroCountAndPremiumDays()
    {
        var writer = new PacketWriter();
        var message = new LoginServerCharacterListMessage(Array.Empty<CharacterListEntry>(), 30);

        message.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteByte((byte)LoginServerOpcode.CharacterList);
        expectedWriter.WriteByte(0);
        expectedWriter.WriteUInt16(30);

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Write_WithMultipleCharacters_EmitsEachEntryThenPremiumDays()
    {
        var writer = new PacketWriter();
        var characters = new[]
        {
            MakeEntry("Hero", "Antica", 0x0100007Fu, 7172),
            MakeEntry("Villain", "Thais", 0x0200007Fu, 7173)
        };
        var message = new LoginServerCharacterListMessage(characters, 15);

        message.Write(writer);

        var expectedWriter = new PacketWriter();
        expectedWriter.WriteByte((byte)LoginServerOpcode.CharacterList);
        expectedWriter.WriteByte(2);
        characters[0].Write(expectedWriter);
        characters[1].Write(expectedWriter);
        expectedWriter.WriteUInt16(15);

        Assert.That(writer.ToArray(), Is.EqualTo(expectedWriter.ToArray()));
    }

    [Test]
    public void Read_WithNoCharacters_ReturnsEmptyListAndPremiumDays()
    {
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteByte(0);
        payloadWriter.WriteUInt16(30);
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = LoginServerCharacterListMessage.Read(LoginServerOpcode.CharacterList, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Characters, Is.Empty);
            Assert.That(message.PremiumDays, Is.EqualTo((ushort)30));
        });
    }

    [Test]
    public void Read_WithMultipleCharacters_ParsesEachEntryAndPremiumDays()
    {
        var characters = new[]
        {
            MakeEntry("Hero", "Antica", 0x0100007Fu, 7172),
            MakeEntry("Villain", "Thais", 0x0200007Fu, 7173)
        };
        var payloadWriter = new PacketWriter();
        payloadWriter.WriteByte((byte)characters.Length);
        foreach (var character in characters)
        {
            character.Write(payloadWriter);
        }

        payloadWriter.WriteUInt16(15);
        var reader = new PacketReader(payloadWriter.ToArray());

        var message = LoginServerCharacterListMessage.Read(LoginServerOpcode.CharacterList, reader);

        Assert.Multiple(() =>
        {
            Assert.That(message.Characters, Has.Count.EqualTo(2));
            Assert.That(message.Characters[0].Name, Is.EqualTo("Hero"));
            Assert.That(message.Characters[1].Name, Is.EqualTo("Villain"));
            Assert.That(message.PremiumDays, Is.EqualTo((ushort)15));
        });
    }

    [Test]
    public void Read_WithTruncatedPayload_ThrowsProtocolException()
    {
        var reader = new PacketReader(new byte[] { 1 });

        Assert.Throws<ProtocolException>(() => LoginServerCharacterListMessage.Read(LoginServerOpcode.CharacterList, reader));
    }

    [Test]
    public void Opcode_IsCharacterList()
    {
        var message = new LoginServerCharacterListMessage(Array.Empty<CharacterListEntry>(), 0);

        Assert.That(message.Opcode, Is.EqualTo((byte)LoginServerOpcode.CharacterList));
    }

    [Test]
    public void Constructor_WithNullCharacters_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginServerCharacterListMessage(null!, 0));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        var message = new LoginServerCharacterListMessage(Array.Empty<CharacterListEntry>(), 0);

        Assert.Throws<ArgumentNullException>(() => message.Write(null!));
    }

    [Test]
    public void Read_WithNullReader_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => LoginServerCharacterListMessage.Read(LoginServerOpcode.CharacterList, null!));
    }
}
