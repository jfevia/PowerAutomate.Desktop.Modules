// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Moderation;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Moderation;

[TestFixture]
public class ModerationMessageReadersTests
{
    [Test]
    public void RegisterTo_RegistersEveryModerationOpcode()
    {
        var registry = new GameServerMessageRegistry();

        ModerationMessageReaders.RegisterTo(registry);

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.RuleViolationChannel), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.RuleViolationRemove), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.RuleViolationCancel), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.RuleViolationLock), Is.True);
        });
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ModerationMessageReaders.RegisterTo(null!));
    }

    [Test]
    public void ReadAll_WithRuleViolationLockOpcode_DecodesPayloadlessMessage()
    {
        var registry = new GameServerMessageRegistry();
        ModerationMessageReaders.RegisterTo(registry);

        var messages = registry.ReadAll(new byte[] { (byte)GameServerOpcode.RuleViolationLock });

        Assert.That(messages[0], Is.InstanceOf<PayloadlessMessage>());
    }
}
