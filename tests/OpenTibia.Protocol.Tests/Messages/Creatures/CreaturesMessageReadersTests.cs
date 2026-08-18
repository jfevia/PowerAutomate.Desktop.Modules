// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Creatures;

[TestFixture]
public class CreaturesMessageReadersTests
{
    [Test]
    public void RegisterTo_RegistersEveryCreatureOpcode()
    {
        var registry = new GameServerMessageRegistry();

        CreaturesMessageReaders.RegisterTo(registry);

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.CreatureHealth), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CreatureLight), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CreatureOutfit), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CreatureSpeed), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CreatureSkull), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CreatureParty), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CreatureSquare), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CreatureUnpass), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.ChooseOutfit), Is.True);
        });
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CreaturesMessageReaders.RegisterTo(null!));
    }
}
