// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Quest;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Quest;

[TestFixture]
public class QuestMessageReadersTests
{
    [Test]
    public void RegisterTo_RegistersEveryQuestOpcode()
    {
        var registry = new GameServerMessageRegistry();

        QuestMessageReaders.RegisterTo(registry);

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.QuestLog), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.QuestLine), Is.True);
        });
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => QuestMessageReaders.RegisterTo(null!));
    }
}
