// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;

/// <summary>
/// Server notice that an item was added to an open container (s2c 0x70).
/// </summary>
public sealed class GameServerCreateContainerMessage : IProtocolMessage
{
    public GameServerCreateContainerMessage(byte containerIndex, ItemStack item)
    {
        ContainerIndex = containerIndex;
        Item = item ?? throw new ArgumentNullException(nameof(item));
    }

    public byte Opcode => (byte)GameServerOpcode.CreateContainer;

    public byte ContainerIndex { get; }

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

        var containerIndex = reader.ReadByte();
        var item = ItemStackCodec.Read(reader, itemTypes);
        return new GameServerCreateContainerMessage(containerIndex, item);
    }
}
