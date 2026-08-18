// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Trade;

[TestFixture]
public class TradeMessageReadersTests
{
    private static readonly FakeItemTypeProvider ItemTypes = new FakeItemTypeProvider();

    [Test]
    public void RegisterTo_WithDefaultProvider_RegistersEveryTradeOpcode()
    {
        var registry = new GameServerMessageRegistry();

        TradeMessageReaders.RegisterTo(registry);

        AssertAllRegistered(registry);
    }

    [Test]
    public void RegisterTo_WithItemTypesProvider_RegistersEveryTradeOpcode()
    {
        var registry = new GameServerMessageRegistry();

        TradeMessageReaders.RegisterTo(registry, ItemTypes);

        AssertAllRegistered(registry);
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => TradeMessageReaders.RegisterTo(null!));
    }

    [Test]
    public void RegisterTo_WithNullRegistryAndItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => TradeMessageReaders.RegisterTo(null!, ItemTypes));
    }

    [Test]
    public void RegisterTo_WithNullItemTypes_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => TradeMessageReaders.RegisterTo(new GameServerMessageRegistry(), null!));
    }

    [Test]
    public void ReadAll_WithCloseNpcTradeOpcode_DecodesPayloadlessMessage()
    {
        var registry = new GameServerMessageRegistry();
        TradeMessageReaders.RegisterTo(registry);

        var messages = registry.ReadAll(new byte[] { (byte)GameServerOpcode.CloseNpcTrade });

        Assert.That(messages[0], Is.InstanceOf<PayloadlessMessage>());
    }

    [Test]
    public void ReadAll_WithCloseTradeOpcode_DecodesPayloadlessMessage()
    {
        var registry = new GameServerMessageRegistry();
        TradeMessageReaders.RegisterTo(registry);

        var messages = registry.ReadAll(new byte[] { (byte)GameServerOpcode.CloseTrade });

        Assert.That(messages[0], Is.InstanceOf<PayloadlessMessage>());
    }

    private static void AssertAllRegistered(GameServerMessageRegistry registry)
    {
        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.OpenNpcTrade), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.PlayerGoods), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CloseNpcTrade), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.OwnTrade), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CounterTrade), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.CloseTrade), Is.True);
        });
    }
}
