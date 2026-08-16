// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Chat;

[TestFixture]
public class ChatMessageReadersTests
{
    [Test]
    public void RegisterTo_RegistersEveryChatOpcode()
    {
        var registry = new GameServerMessageRegistry();

        ChatMessageReaders.RegisterTo(registry);

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.TextMessage), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.Talk), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.Channels), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.OpenChannel), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CloseChannel), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.OpenPrivateChannel), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.OpenOwnChannel), Is.True);
        });
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ChatMessageReaders.RegisterTo(null!));
    }
}
