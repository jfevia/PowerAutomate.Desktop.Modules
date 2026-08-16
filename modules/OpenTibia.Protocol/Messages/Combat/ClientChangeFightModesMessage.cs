// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Combat;

/// <summary>
/// Client request setting the fight, chase and secure modes (opcode 0xA0).
/// </summary>
public sealed class ClientChangeFightModesMessage : IClientMessage
{
    public ClientChangeFightModesMessage(FightMode fightMode, ChaseMode chaseMode, SecureMode secureMode)
    {
        FightMode = fightMode;
        ChaseMode = chaseMode;
        SecureMode = secureMode;
    }

    public FightMode FightMode { get; }

    public ChaseMode ChaseMode { get; }

    public SecureMode SecureMode { get; }

    public byte Opcode => (byte)ClientOpcode.ChangeFightModes;

    public void Write(PacketWriter writer)
    {
        MessageWriter.WriteOpcode(writer, Opcode);
        writer.WriteByte((byte)FightMode);
        writer.WriteByte((byte)ChaseMode);
        writer.WriteByte((byte)SecureMode);
    }
}
