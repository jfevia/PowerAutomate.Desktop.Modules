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
public class ClientLoginRequestMessageTests
{
    private static readonly uint[] XteaKey = { 0x11223344u, 0x55667788u, 0x99AABBCCu, 0xDDEEFF00u };

    private static ClientLoginRequestMessage CreateMessage(string accountName = "account1", string password = "secret")
    {
        return new ClientLoginRequestMessage(7, 860, 0x01020304u, 0x05060708u, 0x090A0B0Cu, XteaKey, accountName, password);
    }

    [Test]
    public void Write_EmitsOpcodeAndHeaderFieldsInOrder()
    {
        var writer = new PacketWriter();

        CreateMessage().Write(writer);

        var bytes = writer.ToArray();

        var expectedHeaderWriter = new PacketWriter();
        expectedHeaderWriter.WriteByte((byte)ClientOpcode.LoginServerRequest);
        expectedHeaderWriter.WriteUInt16(7);
        expectedHeaderWriter.WriteUInt16(860);
        expectedHeaderWriter.WriteUInt32(0x01020304u);
        expectedHeaderWriter.WriteUInt32(0x05060708u);
        expectedHeaderWriter.WriteUInt32(0x090A0B0Cu);
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
    public void Write_EncryptsRsaBlockContainingKeyAndCredentials()
    {
        var writer = new PacketWriter();

        CreateMessage("acct", "pw").Write(writer);

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
            Assert.That(reader.ReadString(), Is.EqualTo("acct"));
            Assert.That(reader.ReadString(), Is.EqualTo("pw"));
            Assert.That(reader.ReadBytes(reader.Remaining), Is.All.EqualTo(0));
        });
    }

    [Test]
    public void Opcode_IsLoginServerRequest()
    {
        Assert.That(CreateMessage().Opcode, Is.EqualTo((byte)ClientOpcode.LoginServerRequest));
    }

    [Test]
    public void Constructor_WithNullXteaKey_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ClientLoginRequestMessage(1, 860, 0, 0, 0, null!, "a", "p"));
    }

    [Test]
    public void Constructor_WithWrongSizedXteaKey_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new ClientLoginRequestMessage(1, 860, 0, 0, 0, new uint[] { 1, 2, 3 }, "a", "p"));
    }

    [Test]
    public void Constructor_WithNullAccountName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ClientLoginRequestMessage(1, 860, 0, 0, 0, XteaKey, null!, "p"));
    }

    [Test]
    public void Constructor_WithNullPassword_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ClientLoginRequestMessage(1, 860, 0, 0, 0, XteaKey, "a", null!));
    }

    [Test]
    public void Write_WithNullWriter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CreateMessage().Write(null!));
    }

    [Test]
    public void Write_WithPlaintextLongerThanRsaBlock_ThrowsArgumentException()
    {
        var message = CreateMessage(new string('a', 200), "p");

        Assert.Throws<ArgumentException>(() => message.Write(new PacketWriter()));
    }
}
