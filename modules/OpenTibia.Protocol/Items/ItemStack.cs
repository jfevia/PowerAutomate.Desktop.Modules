// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Items;

/// <summary>
/// Equipment slots in their 8.60 wire order.
/// </summary>
public enum InventorySlot : byte
{
    Head = 1,
    Necklace = 2,
    Backpack = 3,
    Armor = 4,
    Right = 5,
    Left = 6,
    Legs = 7,
    Feet = 8,
    Ring = 9,
    Ammo = 10
}

/// <summary>
/// Tells the codec whether an item carries an extra count or fluid byte.
/// </summary>
/// <remarks>
/// Supplied by the caller so no items.otb database has to ship with the module.
/// </remarks>
public interface IItemTypeProvider
{
    bool IsStackable(ushort itemId);

    bool IsFluidContainer(ushort itemId);

    bool IsSplash(ushort itemId);
}

/// <summary>
/// An item as it appears on the wire.
/// </summary>
public sealed class ItemStack
{
    public ItemStack(ushort itemId, byte extra)
    {
        ItemId = itemId;
        Extra = extra;
    }

    public ushort ItemId { get; }

    /// <summary>
    /// Count for stackable items, fluid type for containers and splashes, otherwise zero.
    /// </summary>
    public byte Extra { get; }

    public override string ToString()
    {
        return $"item {ItemId} x{Extra}";
    }
}

/// <summary>
/// Reads and writes an <see cref="ItemStack" />, consulting the type provider for the optional byte.
/// </summary>
public static class ItemStackCodec
{
    public static ItemStack Read(PacketReader reader, IItemTypeProvider itemTypes)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        var itemId = reader.ReadUInt16();
        byte extra = 0;
        if (HasExtraByte(itemId, itemTypes))
        {
            extra = reader.ReadByte();
        }

        return new ItemStack(itemId, extra);
    }

    public static void Write(PacketWriter writer, ItemStack stack, IItemTypeProvider itemTypes)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (stack == null)
        {
            throw new ArgumentNullException(nameof(stack));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        writer.WriteUInt16(stack.ItemId);
        if (HasExtraByte(stack.ItemId, itemTypes))
        {
            writer.WriteByte(stack.Extra);
        }
    }

    private static bool HasExtraByte(ushort itemId, IItemTypeProvider itemTypes)
    {
        return itemTypes.IsStackable(itemId)
               || itemTypes.IsFluidContainer(itemId)
               || itemTypes.IsSplash(itemId);
    }
}
