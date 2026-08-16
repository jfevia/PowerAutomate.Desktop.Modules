// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;

/// <summary>
/// Server notice clearing an equipped inventory slot (s2c 0x79).
/// </summary>
public sealed class GameServerDeleteInventoryMessage : IProtocolMessage
{
    public GameServerDeleteInventoryMessage(byte slot)
    {
        Slot = slot;
    }

    public byte Opcode => (byte)GameServerOpcode.DeleteInventory;

    /// <summary>
    /// The <see cref="InventorySlot" /> value being cleared.
    /// </summary>
    public byte Slot { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new GameServerDeleteInventoryMessage(reader.ReadByte());
    }
}
