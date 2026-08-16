// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Player;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Player;

[TestFixture]
public class PlayerMessageReadersTests
{
    [Test]
    public void RegisterTo_RegistersEveryPlayerOpcode()
    {
        var registry = new GameServerMessageRegistry();

        PlayerMessageReaders.RegisterTo(registry);

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.PlayerData), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.PlayerSkills), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.PlayerState), Is.True);
        });
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerMessageReaders.RegisterTo(null!));
    }
}
