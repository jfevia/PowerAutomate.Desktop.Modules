// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Movement;

[TestFixture]
public class MovementMessageReadersTests
{
    [Test]
    public void RegisterTo_RegistersEveryMovementOpcode()
    {
        var registry = new GameServerMessageRegistry();

        MovementMessageReaders.RegisterTo(registry);

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.CancelWalk), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.MoveCreature), Is.True);
        });
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MovementMessageReaders.RegisterTo(null!));
    }
}
