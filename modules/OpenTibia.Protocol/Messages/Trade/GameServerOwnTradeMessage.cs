// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;

/// <summary>
/// Server notice of the local player's own trade offer (s2c 0x7D).
/// </summary>
public sealed class GameServerOwnTradeMessage : IProtocolMessage
{
    public GameServerOwnTradeMessage(string playerName, IReadOnlyList<ItemStack> items)
    {
        PlayerName = playerName ?? throw new ArgumentNullException(nameof(playerName));
        Items = items ?? throw new ArgumentNullException(nameof(items));
    }

    public byte Opcode => (byte)GameServerOpcode.OwnTrade;

    public string PlayerName { get; }

    public IReadOnlyList<ItemStack> Items { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader, IItemTypeProvider itemTypes)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        var playerName = reader.ReadString();
        var itemCount = reader.ReadByte();
        var items = new List<ItemStack>(itemCount);
        for (var index = 0; index < itemCount; index++)
        {
            items.Add(ItemStackCodec.Read(reader, itemTypes));
        }

        return new GameServerOwnTradeMessage(playerName, items);
    }
}
