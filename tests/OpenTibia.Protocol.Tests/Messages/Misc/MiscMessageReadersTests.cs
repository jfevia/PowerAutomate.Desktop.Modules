// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Misc;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Misc;

[TestFixture]
public class MiscMessageReadersTests
{
    [Test]
    public void RegisterTo_RegistersReLoginWindowOpcode()
    {
        var registry = new GameServerMessageRegistry();

        MiscMessageReaders.RegisterTo(registry);

        Assert.That(registry.IsRegistered(GameServerOpcode.ReLoginWindow), Is.True);
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MiscMessageReaders.RegisterTo(null!));
    }

    [Test]
    public void ReadAll_WithReLoginWindowOpcode_DecodesPayloadlessMessage()
    {
        var registry = new GameServerMessageRegistry();
        MiscMessageReaders.RegisterTo(registry);

        var messages = registry.ReadAll(new byte[] { (byte)GameServerOpcode.ReLoginWindow });

        Assert.That(messages[0], Is.InstanceOf<PayloadlessMessage>());
    }
}
