// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Login;

[TestFixture]
public class ClientGameEnterMessageTests
{
    private static readonly uint[] XteaKey = { 0xAABBCCDDu, 0x11223344u, 0x55667788u, 0x99AA55AAu };

    private static ClientGameEnterMessage CreateMessage(
        bool isGamemaster = false,
        string accountName = "account1",
        string characterName = "Hero",
        string password = "secret")
    {
        return new ClientGameEnterMessage(7, 860, XteaKey, isGamemaster, accountName, characterName, password, 0xCAFEBABEu, 0x42);
    }

    [Test]
    public void Write_EmitsOpcodeAndHeaderFieldsInOrder()
    {
        var writer = new PacketWriter();

        CreateMessage().Write(writer);

        var bytes = writer.ToArray();

        var expectedHeaderWriter = new PacketWriter();
        expectedHeaderWriter.WriteByte((byte)ClientOpcode.EnterGame);
        expectedHeaderWriter.WriteUInt16(7);
        expectedHeaderWriter.WriteUInt16(860);
        var expectedHeader = expectedHeaderWriter.ToArray();

        var actualHeader = new byte[expectedHeader.Length];
        Array.Copy(bytes, actualHeader, expectedHeader.Length);

        Assert.Multiple(() =>
        {
            Assert.That(bytes.Length, Is.EqualTo(expectedHeader.Length + RsaKeyExchange.BlockSize));
            Assert.That(actualHeader, Is.EqualTo(expectedHeader));
        });
    }

    [Test]
    public void Write_WithGamemasterFlagTrue_EncryptsRsaBlockWithGamemasterByteSet()
    {
        AssertRsaBlockRoundTrips(isGamemaster: true, expectedGamemasterByte: 1);
    }

    [Test]
    public void Write_WithGamemasterFlagFalse_EncryptsRsaBlockWithGamemasterByteClear()
    {
        AssertRsaBlockRoundTrips(isGamemaster: false, expectedGamemasterByte: 0);
    }

    private static void AssertRsaBlockRoundTrips(bool isGamemaster, byte expectedGamemasterByte)
    {
        var writer = new PacketWriter();

        CreateMessage(isGamemaster, "acct", "Char", "pw").Write(writer);

        var bytes = writer.ToArray();
        var rsaBlock = new byte[RsaKeyExchange.BlockSize];
        Array.Copy(bytes, bytes.Length - RsaKeyExchange.BlockSize, rsaBlock, 0, RsaKeyExchange.BlockSize);

        var plaintext = RsaKeyExchange.Decrypt(rsaBlock);
        var reader = new PacketReader(plaintext);

        Assert.Multiple(() =>
        {
            Assert.That(reader.ReadByte(), Is.EqualTo(0));
            Assert.That(reader.ReadUInt32(), Is.EqualTo(XteaKey[0]));
            Assert.That(reader.ReadUInt32(), Is.EqualTo(XteaKey[1]));
            Assert.That(reader.ReadUInt32(), Is.EqualTo(XteaKey[2]));
            Assert.That(reader.ReadUInt32(), Is.EqualTo(XteaKey[3]));
            Assert.That(reader.ReadByte(), Is.EqualTo(expectedGamemasterByte));
            Assert.That(reader.ReadString(), Is.EqualTo("acct"));
            Assert.That(reader.ReadString(), Is.EqualTo("Char"));
            Assert.That(reader.ReadString(), Is.EqualTo("pw"));
            Assert.That(reader.ReadUInt32(), Is.EqualTo(0xCAFEBABEu));
            Assert.That(reader.ReadByte(), Is.EqualTo(0x42));
            Assert.That(reader.ReadBytes(reader.Remaining), Is.All.EqualTo(0));
        });
    }

    [Test]
    public void Opcode_IsEnterGame()
    {
        Assert.That(CreateMessage().Opcode, Is.EqualTo((byte)ClientOpcode.EnterGame));
    }

    [Test]
    public void Constructor_WithNullXteaKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ClientGameEnterMessage(1, 860, null!, false, "a", "c", "p", 0, 0));
    }

    [Test]
    public void Constructor_WithWrongSizedXteaKey_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new ClientGameEnterMessage(1, 860, new uint[] { 1, 2 }, false, "a", "c", "p", 0, 0));
    }

    [Test]
    public void Constructor_WithNullAccountName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ClientGameEnterMessage(1, 860, XteaKey, false, null!, "c", "p", 0, 0));
    }

    [Test]
    public void Constructor_WithNullCharacterName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ClientGameEnterMessage(1, 860, XteaKey, false, "a", null!, "p", 0, 0));
    }

    [Test]
    public void Constructor_WithNullPassword_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ClientGameEnterMessage(1, 860, XteaKey, false, "a", "c", null!, 0, 0));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CreateMessage().Write(null!));
    }

    [Test]
    public void Write_WithPlaintextLongerThanRsaBlock_ThrowsArgumentException()
    {
        var message = CreateMessage(false, new string('a', 200), "Char", "p");

        Assert.Throws<ArgumentException>(() => message.Write(new PacketWriter()));
    }
}
