// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Server-broadcast chat message (s2c 0xAA); exactly one type-dependent field is present per <see cref="Type"/>.
/// </summary>
public sealed class GameServerTalkMessage : IProtocolMessage
{
    private static readonly HashSet<SpeakType> PositionSpeakTypes = new HashSet<SpeakType>
    {
        SpeakType.Say, SpeakType.Whisper, SpeakType.Yell,
        SpeakType.MonsterSay, SpeakType.MonsterYell, SpeakType.PrivateNameToPlayer
    };

    private static readonly HashSet<SpeakType> ChannelIdSpeakTypes = new HashSet<SpeakType>
    {
        SpeakType.ChannelYellow, SpeakType.ChannelRed, SpeakType.ChannelRedAnonymous,
        SpeakType.ChannelOrange, SpeakType.ChannelWhite
    };

    public GameServerTalkMessage(
        uint statementId,
        string speakerName,
        ushort speakerLevel,
        SpeakType type,
        Position? speakerPosition,
        ushort? channelId,
        uint? ruleViolationChannelTimeDelta,
        string text)
    {
        StatementId = statementId;
        SpeakerName = speakerName ?? throw new ArgumentNullException(nameof(speakerName));
        SpeakerLevel = speakerLevel;
        Type = type;
        SpeakerPosition = speakerPosition;
        ChannelId = channelId;
        RuleViolationChannelTimeDelta = ruleViolationChannelTimeDelta;
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public byte Opcode => (byte)GameServerOpcode.Talk;

    public uint StatementId { get; }

    public string SpeakerName { get; }

    public ushort SpeakerLevel { get; }

    public SpeakType Type { get; }

    /// <summary>
    /// Present for say/whisper/yell/monster/private-name-to-player speak types.
    /// </summary>
    public Position? SpeakerPosition { get; }

    /// <summary>
    /// Present for channel-yellow/red/red-anonymous/orange/white speak types.
    /// </summary>
    public ushort? ChannelId { get; }

    /// <summary>
    /// Present only for the rule-violation-channel speak type.
    /// </summary>
    public uint? RuleViolationChannelTimeDelta { get; }

    public string Text { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var statementId = reader.ReadUInt32();
        var speakerName = reader.ReadString();
        var speakerLevel = reader.ReadUInt16();
        var type = (SpeakType)reader.ReadByte();

        Position? speakerPosition = null;
        ushort? channelId = null;
        uint? ruleViolationChannelTimeDelta = null;
        if (PositionSpeakTypes.Contains(type))
        {
            speakerPosition = PositionCodec.Read(reader);
        }
        else if (ChannelIdSpeakTypes.Contains(type))
        {
            channelId = reader.ReadUInt16();
        }
        else if (type == SpeakType.RuleViolationChannel)
        {
            ruleViolationChannelTimeDelta = reader.ReadUInt32();
        }

        var text = reader.ReadString();
        return new GameServerTalkMessage(statementId, speakerName, speakerLevel, type, speakerPosition, channelId, ruleViolationChannelTimeDelta, text);
    }
}
