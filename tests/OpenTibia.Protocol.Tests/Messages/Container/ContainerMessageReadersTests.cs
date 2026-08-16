// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Container;

[TestFixture]
public class ContainerMessageReadersTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void RegisterTo_WithDefaultProvider_RegistersEveryContainerOpcode()
    {
        var registry = new GameServerMessageRegistry();

        ContainerMessageReaders.RegisterTo(registry);

        AssertAllRegistered(registry);
    }

    [Test]
    public void RegisterTo_WithItemTypesProvider_RegistersEveryContainerOpcode()
    {
        var registry = new GameServerMessageRegistry();

        ContainerMessageReaders.RegisterTo(registry, ItemTypes);

        AssertAllRegistered(registry);
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ContainerMessageReaders.RegisterTo(null!));
    }

    [Test]
    public void RegisterTo_WithNullRegistryAndItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ContainerMessageReaders.RegisterTo(null!, ItemTypes));
    }

    [Test]
    public void RegisterTo_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => ContainerMessageReaders.RegisterTo(new GameServerMessageRegistry(), null!));
    }

    /// <summary>
    /// Regression test for the live TFS r3884 capture that first exposed opcode 0x79 as unregistered.
    /// </summary>
    [Test]
    public void ReadAll_WithRealCapturedDeleteAndSetInventoryBytes_DecodesBothMessages()
    {
        var registry = new GameServerMessageRegistry();
        ContainerMessageReaders.RegisterTo(registry);

        var payload = new byte[] { 0x79, 0x01, 0x78, 0x05, 0x68, 0x0B };
        var messages = registry.ReadAll(payload);

        Assert.That(messages, Has.Count.EqualTo(2));
        var deleteInventory = (GameServerDeleteInventoryMessage)messages[0];
        var setInventory = (GameServerSetInventoryMessage)messages[1];
        Assert.Multiple(() =>
        {
            Assert.That(deleteInventory.Slot, Is.EqualTo(1));
            Assert.That(setInventory.Slot, Is.EqualTo(5));
            Assert.That(setInventory.Item.ItemId, Is.EqualTo(2920));
            Assert.That(setInventory.Item.Extra, Is.EqualTo(0));
        });
    }

    private static void AssertAllRegistered(GameServerMessageRegistry registry)
    {
        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.OpenContainer), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CloseContainer), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CreateContainer), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.ChangeInContainer), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.DeleteInContainer), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.SetInventory), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.DeleteInventory), Is.True);
        });
    }
}
