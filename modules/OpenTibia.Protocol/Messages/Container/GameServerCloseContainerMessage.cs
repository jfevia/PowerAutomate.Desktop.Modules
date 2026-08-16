// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;

/// <summary>
/// Server notice closing a container's contents window (s2c 0x6F).
/// </summary>
public sealed class GameServerCloseContainerMessage : IProtocolMessage
{
    public GameServerCloseContainerMessage(byte containerIndex)
    {
        ContainerIndex = containerIndex;
    }

    public byte Opcode => (byte)GameServerOpcode.CloseContainer;

    public byte ContainerIndex { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new GameServerCloseContainerMessage(reader.ReadByte());
    }
}
