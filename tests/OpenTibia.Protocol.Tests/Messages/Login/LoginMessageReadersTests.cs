// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Login;

[TestFixture]
public class LoginMessageReadersTests
{
    private static GameServerMessageRegistry CreateRegisteredRegistry()
    {
        var registry = new GameServerMessageRegistry();
        LoginMessageReaders.RegisterTo(registry);
        return registry;
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => LoginMessageReaders.RegisterTo(null!));
    }

    [Test]
    public void RegisterTo_RegistersEveryLoginOpcode()
    {
        var registry = CreateRegisteredRegistry();

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.Challenge), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.LoginError), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.LoginAdvice), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.LoginWait), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.LoginOrPendingState), Is.True);
        });
    }

    [Test]
    public void RegisterTo_DoesNotRegisterUnrelatedOpcodes()
    {
        var registry = CreateRegisteredRegistry();

        Assert.That(registry.IsRegistered(GameServerOpcode.Ping), Is.False);
    }

    [Test]
    public void ReadAll_DecodesRegisteredChallengeMessage()
    {
        var registry = CreateRegisteredRegistry();
        var writer = new PacketWriter();
        new GameServerChallengeMessage(0x12345678u, 0x42).Write(writer);

        var messages = registry.ReadAll(writer.ToArray());

        Assert.Multiple(() =>
        {
            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(messages[0], Is.InstanceOf<GameServerChallengeMessage>());
            Assert.That(((GameServerChallengeMessage)messages[0]).Timestamp, Is.EqualTo(0x12345678u));
        });
    }

    [Test]
    public void ReadAll_DecodesRegisteredLoginOrPendingStateMessage()
    {
        var registry = CreateRegisteredRegistry();
        var writer = new PacketWriter();
        new GameServerLoginOrPendingStateMessage(7, 50, true, null).Write(writer);

        var messages = registry.ReadAll(writer.ToArray());

        Assert.Multiple(() =>
        {
            Assert.That(messages, Has.Count.EqualTo(1));
            Assert.That(messages[0], Is.InstanceOf<GameServerLoginOrPendingStateMessage>());
            Assert.That(((GameServerLoginOrPendingStateMessage)messages[0]).PlayerId, Is.EqualTo(7u));
        });
    }
}
