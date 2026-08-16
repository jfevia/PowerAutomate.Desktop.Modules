// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;

/// <summary>
/// Server notice opening a container's contents window (s2c 0x6E).
/// </summary>
public sealed class GameServerOpenContainerMessage : IProtocolMessage
{
    public GameServerOpenContainerMessage(
        byte containerIndex,
        ushort containerItemId,
        string containerName,
        byte capacity,
        bool isParentAvailable,
        IReadOnlyList<ItemStack> items)
    {
        ContainerIndex = containerIndex;
        ContainerItemId = containerItemId;
        ContainerName = containerName ?? throw new ArgumentNullException(nameof(containerName));
        Capacity = capacity;
        IsParentAvailable = isParentAvailable;
        Items = items ?? throw new ArgumentNullException(nameof(items));
    }

    public byte Opcode => (byte)GameServerOpcode.OpenContainer;

    public byte ContainerIndex { get; }

    /// <summary>
    /// The container item's own client id, written as a raw id (containers are never stackable/fluid/splash).
    /// </summary>
    public ushort ContainerItemId { get; }

    public string ContainerName { get; }

    public byte Capacity { get; }

    public bool IsParentAvailable { get; }

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

        var containerIndex = reader.ReadByte();
        var containerItemId = reader.ReadUInt16();
        var containerName = reader.ReadString();
        var capacity = reader.ReadByte();
        var isParentAvailable = reader.ReadByte() != 0;
        var itemCount = reader.ReadByte();
        var items = new List<ItemStack>(itemCount);
        for (var index = 0; index < itemCount; index++)
        {
            items.Add(ItemStackCodec.Read(reader, itemTypes));
        }

        return new GameServerOpenContainerMessage(containerIndex, containerItemId, containerName, capacity, isParentAvailable, items);
    }
}
