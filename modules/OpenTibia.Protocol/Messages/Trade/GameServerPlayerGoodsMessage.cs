// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;

/// <summary>
/// Server notice of the local player's money and sellable goods in an open NPC shop (s2c 0x7B).
/// </summary>
public sealed class GameServerPlayerGoodsMessage : IProtocolMessage
{
    public GameServerPlayerGoodsMessage(uint money, IReadOnlyList<GoodsEntry> goods)
    {
        Money = money;
        Goods = goods ?? throw new ArgumentNullException(nameof(goods));
    }

    public byte Opcode => (byte)GameServerOpcode.PlayerGoods;

    public uint Money { get; }

    public IReadOnlyList<GoodsEntry> Goods { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var money = reader.ReadUInt32();
        var goodsCount = reader.ReadByte();
        var goods = new List<GoodsEntry>(goodsCount);
        for (var index = 0; index < goodsCount; index++)
        {
            goods.Add(GoodsEntry.Read(reader));
        }

        return new GameServerPlayerGoodsMessage(money, goods);
    }
}
