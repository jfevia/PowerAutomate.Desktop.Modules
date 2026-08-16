// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Effects;

[TestFixture]
public class EffectsMessageReadersTests
{
    [Test]
    public void RegisterTo_RegistersEveryEffectsOpcode()
    {
        var registry = new GameServerMessageRegistry();

        EffectsMessageReaders.RegisterTo(registry);

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.Ambient), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.GraphicalEffect), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.TextEffect), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.MissleEffect), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.ClearTarget), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.TutorialHint), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.AutomapFlag), Is.True);
        });
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => EffectsMessageReaders.RegisterTo(null!));
    }
}
