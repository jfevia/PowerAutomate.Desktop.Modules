// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Social;

/// <summary>
/// Server confirmation that a VIP contact was added, including its current online state (s2c 0xD2).
/// </summary>
public sealed class GameServerVipAddMessage : IProtocolMessage
{
    public GameServerVipAddMessage(uint id, string name, bool isOnline)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsOnline = isOnline;
    }

    public byte Opcode => (byte)GameServerOpcode.VipAdd;

    public uint Id { get; }

    public string Name { get; }

    public bool IsOnline { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var id = reader.ReadUInt32();
        var name = reader.ReadString();
        var isOnline = reader.ReadByte() != 0;
        return new GameServerVipAddMessage(id, name, isOnline);
    }
}
