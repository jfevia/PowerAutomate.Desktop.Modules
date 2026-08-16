// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Creatures;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Effects;

/// <summary>
/// Server notice of the current world/ambient light (s2c 0x82).
/// </summary>
public sealed class GameServerAmbientMessage : IProtocolMessage
{
    public GameServerAmbientMessage(LightInfo light)
    {
        Light = light;
    }

    public byte Opcode => (byte)GameServerOpcode.Ambient;

    public LightInfo Light { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new GameServerAmbientMessage(LightInfo.Read(reader));
    }
}
