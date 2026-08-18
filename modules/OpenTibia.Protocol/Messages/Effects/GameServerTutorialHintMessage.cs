// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// Server hint pointing the player at a tutorial topic (s2c 0xDC).
/// </summary>
public sealed class GameServerTutorialHintMessage : IProtocolMessage
{
    public GameServerTutorialHintMessage(byte tutorialId)
    {
        TutorialId = tutorialId;
    }

    public byte Opcode => (byte)GameServerOpcode.TutorialHint;

    public byte TutorialId { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new GameServerTutorialHintMessage(reader.ReadByte());
    }
}
