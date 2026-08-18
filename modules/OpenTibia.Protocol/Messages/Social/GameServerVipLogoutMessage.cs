// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Social;

/// <summary>
/// Server notice that a VIP contact logged out (s2c 0xD4).
/// </summary>
public sealed class GameServerVipLogoutMessage : IProtocolMessage
{
    public GameServerVipLogoutMessage(uint id)
    {
        Id = id;
    }

    public byte Opcode => (byte)GameServerOpcode.VipLogout;

    public uint Id { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var id = reader.ReadUInt32();
        return new GameServerVipLogoutMessage(id);
    }
}
