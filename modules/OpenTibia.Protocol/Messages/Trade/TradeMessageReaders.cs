// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;

/// <summary>
/// Registers the NPC and player trade notice message readers.
/// </summary>
public static class TradeMessageReaders
{
    /// <summary>
    /// Registers every opcode using <see cref="DefaultItemTypeProvider" />, since no items.otb ships with this module.
    /// </summary>
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        RegisterTo(registry, new DefaultItemTypeProvider());
    }

    public static void RegisterTo(GameServerMessageRegistry registry, IItemTypeProvider itemTypes)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        registry.Register(GameServerOpcode.OpenNpcTrade, GameServerOpenNpcTradeMessage.Read);
        registry.Register(GameServerOpcode.PlayerGoods, GameServerPlayerGoodsMessage.Read);
        registry.Register(GameServerOpcode.CloseNpcTrade, (opcode, reader) => new PayloadlessMessage(opcode));
        registry.Register(GameServerOpcode.OwnTrade, (opcode, reader) => GameServerOwnTradeMessage.Read(opcode, reader, itemTypes));
        registry.Register(GameServerOpcode.CounterTrade, (opcode, reader) => GameServerCounterTradeMessage.Read(opcode, reader, itemTypes));
        registry.Register(GameServerOpcode.CloseTrade, (opcode, reader) => new PayloadlessMessage(opcode));
    }
}
