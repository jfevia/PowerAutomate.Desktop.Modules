// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;

/// <summary>
/// Server notice setting an equipped inventory slot's item (s2c 0x78).
/// </summary>
public sealed class GameServerSetInventoryMessage : IProtocolMessage
{
    public GameServerSetInventoryMessage(byte slot, ItemStack item)
    {
        Slot = slot;
        Item = item ?? throw new ArgumentNullException(nameof(item));
    }

    public byte Opcode => (byte)GameServerOpcode.SetInventory;

    /// <summary>
    /// The <see cref="InventorySlot" /> value being set.
    /// </summary>
    public byte Slot { get; }

    public ItemStack Item { get; }

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

        var slot = reader.ReadByte();
        var item = ItemStackCodec.Read(reader, itemTypes);
        return new GameServerSetInventoryMessage(slot, item);
    }
}
