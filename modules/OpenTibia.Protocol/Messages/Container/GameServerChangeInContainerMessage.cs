// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;

/// <summary>
/// Server notice that an item within an open container changed (s2c 0x71).
/// </summary>
public sealed class GameServerChangeInContainerMessage : IProtocolMessage
{
    public GameServerChangeInContainerMessage(byte containerIndex, byte slot, ItemStack item)
    {
        ContainerIndex = containerIndex;
        Slot = slot;
        Item = item ?? throw new ArgumentNullException(nameof(item));
    }

    public byte Opcode => (byte)GameServerOpcode.ChangeInContainer;

    public byte ContainerIndex { get; }

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

        var containerIndex = reader.ReadByte();
        var slot = reader.ReadByte();
        var item = ItemStackCodec.Read(reader, itemTypes);
        return new GameServerChangeInContainerMessage(containerIndex, slot, item);
    }
}
