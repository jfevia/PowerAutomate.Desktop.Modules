// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Trade;

/// <summary>
/// One item offered in an NPC shop's stock, within a <see cref="GameServerOpenNpcTradeMessage" />.
/// </summary>
public sealed class ShopItem
{
    public ShopItem(ushort clientId, byte subType, string name, uint weightHundredths, uint buyPrice, uint sellPrice)
    {
        ClientId = clientId;
        SubType = subType;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        WeightHundredths = weightHundredths;
        BuyPrice = buyPrice;
        SellPrice = sellPrice;
    }

    public ushort ClientId { get; }

    /// <summary>
    /// The subtype/count/fluid-type byte, always present regardless of whether the item is stackable.
    /// </summary>
    public byte SubType { get; }

    public string Name { get; }

    public uint WeightHundredths { get; }

    public uint BuyPrice { get; }

    public uint SellPrice { get; }

    public static ShopItem Read(PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var clientId = reader.ReadUInt16();
        var subType = reader.ReadByte();
        var name = reader.ReadString();
        var weightHundredths = reader.ReadUInt32();
        var buyPrice = reader.ReadUInt32();
        var sellPrice = reader.ReadUInt32();
        return new ShopItem(clientId, subType, name, weightHundredths, buyPrice, sellPrice);
    }
}
