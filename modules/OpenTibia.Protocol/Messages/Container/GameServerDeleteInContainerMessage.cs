// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;

/// <summary>
/// Server notice that an item was removed from an open container (s2c 0x72).
/// </summary>
public sealed class GameServerDeleteInContainerMessage : IProtocolMessage
{
    public GameServerDeleteInContainerMessage(byte containerIndex, byte slot)
    {
        ContainerIndex = containerIndex;
        Slot = slot;
    }

    public byte Opcode => (byte)GameServerOpcode.DeleteInContainer;

    public byte ContainerIndex { get; }

    public byte Slot { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var containerIndex = reader.ReadByte();
        var slot = reader.ReadByte();
        return new GameServerDeleteInContainerMessage(containerIndex, slot);
    }
}
