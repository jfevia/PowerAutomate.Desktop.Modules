// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// A chat message's classification, matching TFS's SpeakClasses enum values for protocol 8.60.
/// </summary>
public enum SpeakType : byte
{
    None = 0x00,
    Say = 0x01,
    Whisper = 0x02,
    Yell = 0x03,
    PrivatePlayerToName = 0x04,
    PrivateNameToPlayer = 0x05,
    Private = 0x06,
    ChannelYellow = 0x07,
    ChannelWhite = 0x08,
    RuleViolationChannel = 0x09,
    RuleViolationAnswer = 0x0A,
    RuleViolationContinue = 0x0B,
    Broadcast = 0x0C,
    ChannelRed = 0x0D,
    PrivateRed = 0x0E,
    ChannelOrange = 0x0F,
    ChannelRedAnonymous = 0x11,
    MonsterSay = 0x13,
    MonsterYell = 0x14
}
