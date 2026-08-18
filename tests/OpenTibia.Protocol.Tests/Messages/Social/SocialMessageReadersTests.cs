// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Social;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Social;

[TestFixture]
public class SocialMessageReadersTests
{
    [Test]
    public void RegisterTo_RegistersEverySocialOpcode()
    {
        var registry = new GameServerMessageRegistry();

        SocialMessageReaders.RegisterTo(registry);

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.VipAdd), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.VipState), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.VipLogout), Is.True);
        });
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => SocialMessageReaders.RegisterTo(null!));
    }
}
