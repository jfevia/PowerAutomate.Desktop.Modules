// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;

/// <summary>
/// Server-opened NPC shop stock (s2c 0x7A).
/// </summary>
public sealed class GameServerOpenNpcTradeMessage : IProtocolMessage
{
    public GameServerOpenNpcTradeMessage(IReadOnlyList<ShopItem> items)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
    }

    public byte Opcode => (byte)GameServerOpcode.OpenNpcTrade;

    public IReadOnlyList<ShopItem> Items { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var itemCount = reader.ReadByte();
        var items = new List<ShopItem>(itemCount);
        for (var index = 0; index < itemCount; index++)
        {
            items.Add(ShopItem.Read(reader));
        }

        return new GameServerOpenNpcTradeMessage(items);
    }
}
