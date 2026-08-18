// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// Client chat message (opcode 0x96); the receiver name and channel id are mutually exclusive.
/// </summary>
public sealed class ClientTalkMessage : IClientMessage
{
    private static readonly HashSet<SpeakType> ReceiverNameSpeakTypes = new HashSet<SpeakType>
    {
        SpeakType.Private, SpeakType.PrivateRed, SpeakType.RuleViolationAnswer
    };

    private static readonly HashSet<SpeakType> ChannelIdSpeakTypes = new HashSet<SpeakType>
    {
        SpeakType.ChannelYellow, SpeakType.ChannelRed, SpeakType.ChannelRedAnonymous
    };

    public ClientTalkMessage(SpeakType type, string? receiverName, ushort? channelId, string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (ReceiverNameSpeakTypes.Contains(type) && receiverName == null)
        {
            throw new ProtocolException($"Speak type {type} requires a receiver name.");
        }

        if (ChannelIdSpeakTypes.Contains(type) && channelId == null)
        {
            throw new ProtocolException($"Speak type {type} requires a channel id.");
        }

        Type = type;
        ReceiverName = receiverName;
        ChannelId = channelId;
        Text = text;
    }

    public SpeakType Type { get; }

    /// <summary>
    /// The intended recipient's name, present only for private speak types.
    /// </summary>
    public string? ReceiverName { get; }

    /// <summary>
    /// The target channel id, present only for channel speak types.
    /// </summary>
    public ushort? ChannelId { get; }

    public string Text { get; }

    public byte Opcode => (byte)ClientOpcode.Talk;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteByte((byte)Type);
        if (ReceiverNameSpeakTypes.Contains(Type))
        {
            writer.WriteString(ReceiverName!);
        }
        else if (ChannelIdSpeakTypes.Contains(Type))
        {
            writer.WriteUInt16(ChannelId!.Value);
        }

        writer.WriteString(Text);
    }
}
