// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Client.Tests.Transport;
using PowerAutomate.Desktop.OpenTibia.Protocol;
using PowerAutomate.Desktop.OpenTibia.Protocol.Cryptography;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Handshake;

[TestFixture]
public class TibiaLoginClientTests
{
    private static readonly uint[] Key = { 1u, 2u, 3u, 4u };
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(5);

    private static LoginOptions Options()
    {
        return new LoginOptions("127.0.0.1", 7171, "account", "secret");
    }

    private static byte[] EncryptedFrame(byte[] payload)
    {
        return FrameCodec.EncodeEncrypted(payload, Key);
    }

    private static byte[] MotdPayload(string text)
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)LoginServerOpcode.Motd);
        writer.WriteString(text);
        return writer.ToArray();
    }

    private static byte[] CharacterListPayload(params string[] names)
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)LoginServerOpcode.CharacterList);
        writer.WriteByte((byte)names.Length);
        foreach (var name in names)
        {
            writer.WriteString(name);
            writer.WriteString("Antica");
            writer.WriteUInt32(0x0100007F);
            writer.WriteUInt16(7172);
        }

        writer.WriteUInt16(42);
        return writer.ToArray();
    }

    private static byte[] ErrorPayload(string text)
    {
        var writer = new PacketWriter();
        writer.WriteByte((byte)LoginServerOpcode.Error);
        writer.WriteString(text);
        return writer.ToArray();
    }

    [Test]
    public void Authenticate_ReturnsMotdCharactersAndPremiumDays()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(EncryptedFrame(MotdPayload("welcome")));
        transport.EnqueueRead(EncryptedFrame(CharacterListPayload("Rook", "Knight")));
        var client = new TibiaLoginClient(transport, () => Key);

        var result = client.Authenticate(Options(), Patience);

        Assert.Multiple(() =>
        {
            Assert.That(result.Motd, Is.EqualTo("welcome"));
            Assert.That(result.Characters.Select(entry => entry.Name), Is.EqualTo(new[] { "Rook", "Knight" }));
            Assert.That(result.PremiumDays, Is.EqualTo(42));
        });
    }

    [Test]
    public void Authenticate_SendsAPlaintextRequestCarryingTheLoginOpcode()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(EncryptedFrame(CharacterListPayload("Rook")));
        var client = new TibiaLoginClient(transport, () => Key);

        client.Authenticate(Options(), Patience);

        var body = transport.Written.Single().Skip(FrameCodec.LengthPrefixSize).ToArray();
        var payload = FrameCodec.DecodePlain(body);

        Assert.That(payload[0], Is.EqualTo((byte)ClientOpcode.LoginServerRequest));
    }

    [Test]
    public void Authenticate_WhenBothMessagesArriveInOneFrame_StillSucceeds()
    {
        var combined = new List<byte>();
        combined.AddRange(MotdPayload("hello"));
        combined.AddRange(CharacterListPayload("Rook"));

        var transport = new FakeSocketTransport();
        transport.EnqueueRead(EncryptedFrame(combined.ToArray()));
        var client = new TibiaLoginClient(transport, () => Key);

        var result = client.Authenticate(Options(), Patience);

        Assert.Multiple(() =>
        {
            Assert.That(result.Motd, Is.EqualTo("hello"));
            Assert.That(result.Characters, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void Authenticate_WhenServerRejects_ThrowsLoginRejected()
    {
        var transport = new FakeSocketTransport();
        transport.EnqueueRead(EncryptedFrame(ErrorPayload("Account name or password is not correct.")));
        var client = new TibiaLoginClient(transport, () => Key);

        var exception = Assert.Throws<LoginRejectedException>(() => client.Authenticate(Options(), Patience))!;

        Assert.That(exception.Message, Does.Contain("not correct"));
    }

    [Test]
    public void Authenticate_WhenReadsTimeOutFirst_StillCompletes()
    {
        var transport = new FakeSocketTransport { TimeoutsBeforeData = 2 };
        transport.EnqueueRead(EncryptedFrame(CharacterListPayload("Rook")));
        var client = new TibiaLoginClient(transport, () => Key);

        var result = client.Authenticate(Options(), Patience);

        Assert.That(result.Characters, Has.Count.EqualTo(1));
    }

    [Test]
    public void Authenticate_WhenServerClosesEarly_ThrowsProtocolException()
    {
        var transport = new FakeSocketTransport();
        var client = new TibiaLoginClient(transport, () => Key);

        Assert.Throws<ProtocolException>(() => client.Authenticate(Options(), Patience));
    }

    [Test]
    public void Authenticate_WhenNothingArrivesInTime_ThrowsTimeout()
    {
        var transport = new FakeSocketTransport();
        var client = new TibiaLoginClient(transport, () => Key);

        Assert.Throws<TimeoutException>(() => client.Authenticate(Options(), TimeSpan.Zero));
    }

    [Test]
    public void Constructor_WithNullTransport_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TibiaLoginClient(null!));
    }

    [Test]
    public void Constructor_WithNullKeyFactory_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TibiaLoginClient(new FakeSocketTransport(), null!));
    }

    [Test]
    public void Constructor_WithDefaultKeyFactory_IsAccepted()
    {
        Assert.That(new TibiaLoginClient(new FakeSocketTransport()), Is.Not.Null);
    }

    [Test]
    public void Authenticate_WithNullOptions_Throws()
    {
        var client = new TibiaLoginClient(new FakeSocketTransport(), () => Key);

        Assert.Throws<ArgumentNullException>(() => client.Authenticate(null!, Patience));
    }
}

[TestFixture]
public class LoginOptionsTests
{
    [Test]
    public void Constructor_KeepsEveryFieldAndDefaultsToProtocol860()
    {
        var options = new LoginOptions("host", 7171, "acct", "pw");

        Assert.Multiple(() =>
        {
            Assert.That(options.Host, Is.EqualTo("host"));
            Assert.That(options.Port, Is.EqualTo(7171));
            Assert.That(options.AccountName, Is.EqualTo("acct"));
            Assert.That(options.Password, Is.EqualTo("pw"));
            Assert.That(options.Version, Is.EqualTo(860));
            Assert.That(options.Os, Is.EqualTo(2));
        });
    }

    [Test]
    public void Constructor_WithNullHost_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginOptions(null!, 1, "a", "p"));
    }

    [Test]
    public void Constructor_WithNullAccountName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginOptions("h", 1, null!, "p"));
    }

    [Test]
    public void Constructor_WithNullPassword_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginOptions("h", 1, "a", null!));
    }

    [TestCase(0)]
    [TestCase(70000)]
    public void Constructor_WithPortOutOfRange_Throws(int port)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LoginOptions("h", port, "a", "p"));
    }
}

[TestFixture]
public class LoginResultTests
{
    [Test]
    public void Constructor_WithNullCharacters_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginResult(null!, "m", 0));
    }

    [Test]
    public void Constructor_WithNullMotd_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginResult(Array.Empty<CharacterListEntry>(), null!, 0));
    }
}

[TestFixture]
public class XteaKeyGeneratorTests
{
    [Test]
    public void Generate_ProducesFourWords()
    {
        Assert.That(XteaKeyGenerator.Generate(), Has.Length.EqualTo(XteaCipher.KeyLength));
    }

    [Test]
    public void Generate_DoesNotReturnAnAllZeroKey()
    {
        Assert.That(XteaKeyGenerator.Generate(), Has.Some.Not.Zero);
    }
}

[TestFixture]
public class LoginServerMessageDispatcherTests
{
    [Test]
    public void ReadAll_WithUnknownOpcode_ThrowsProtocolException()
    {
        Assert.Throws<ProtocolException>(() => LoginServerMessageDispatcher.ReadAll(new byte[] { 0xEE }));
    }

    [Test]
    public void ReadAll_WithNullPayload_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => LoginServerMessageDispatcher.ReadAll(null!));
    }

    [Test]
    public void ReadAll_WithEmptyPayload_ReturnsNothing()
    {
        Assert.That(LoginServerMessageDispatcher.ReadAll(Array.Empty<byte>()), Is.Empty);
    }
}
