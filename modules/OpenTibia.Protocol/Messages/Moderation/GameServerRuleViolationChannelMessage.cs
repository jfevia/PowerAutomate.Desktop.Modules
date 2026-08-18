// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Moderation;

/// <summary>
/// Server notice identifying the rule-violations report channel (s2c 0xAE).
/// </summary>
public sealed class GameServerRuleViolationChannelMessage : IProtocolMessage
{
    public GameServerRuleViolationChannelMessage(ushort channelId)
    {
        ChannelId = channelId;
    }

    public byte Opcode => (byte)GameServerOpcode.RuleViolationChannel;

    public ushort ChannelId { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new GameServerRuleViolationChannelMessage(reader.ReadUInt16());
    }
}
