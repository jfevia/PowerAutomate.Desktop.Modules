// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Chat;

[TestFixture]
public class SpeakTypeTests
{
    [TestCase(SpeakType.None, (byte)0x00)]
    [TestCase(SpeakType.Say, (byte)0x01)]
    [TestCase(SpeakType.Whisper, (byte)0x02)]
    [TestCase(SpeakType.Yell, (byte)0x03)]
    [TestCase(SpeakType.PrivatePlayerToName, (byte)0x04)]
    [TestCase(SpeakType.PrivateNameToPlayer, (byte)0x05)]
    [TestCase(SpeakType.Private, (byte)0x06)]
    [TestCase(SpeakType.ChannelYellow, (byte)0x07)]
    [TestCase(SpeakType.ChannelWhite, (byte)0x08)]
    [TestCase(SpeakType.RuleViolationChannel, (byte)0x09)]
    [TestCase(SpeakType.RuleViolationAnswer, (byte)0x0A)]
    [TestCase(SpeakType.RuleViolationContinue, (byte)0x0B)]
    [TestCase(SpeakType.Broadcast, (byte)0x0C)]
    [TestCase(SpeakType.ChannelRed, (byte)0x0D)]
    [TestCase(SpeakType.PrivateRed, (byte)0x0E)]
    [TestCase(SpeakType.ChannelOrange, (byte)0x0F)]
    [TestCase(SpeakType.ChannelRedAnonymous, (byte)0x11)]
    [TestCase(SpeakType.MonsterSay, (byte)0x13)]
    [TestCase(SpeakType.MonsterYell, (byte)0x14)]
    public void RawValue_MatchesThe860WireValue(SpeakType type, byte expected)
    {
        Assert.That((byte)type, Is.EqualTo(expected));
    }
}
