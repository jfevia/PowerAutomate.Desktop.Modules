// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Social;

/// <summary>
/// Server notice that a VIP contact logged in (s2c 0xD3); the wire carries only the contact id.
/// </summary>
public sealed class GameServerVipStateMessage : IProtocolMessage
{
    public GameServerVipStateMessage(uint id)
    {
        Id = id;
    }

    public byte Opcode => (byte)GameServerOpcode.VipState;

    public uint Id { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var id = reader.ReadUInt32();
        return new GameServerVipStateMessage(id);
    }
}
