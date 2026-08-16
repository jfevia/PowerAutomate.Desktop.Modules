// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Server notice opening a private message channel with another player (s2c 0xAD).
/// </summary>
public sealed class GameServerOpenPrivateChannelMessage : IProtocolMessage
{
    public GameServerOpenPrivateChannelMessage(string receiverName)
    {
        ReceiverName = receiverName ?? throw new ArgumentNullException(nameof(receiverName));
    }

    public byte Opcode => (byte)GameServerOpcode.OpenPrivateChannel;

    public string ReceiverName { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var receiverName = reader.ReadString();
        return new GameServerOpenPrivateChannelMessage(receiverName);
    }
}
