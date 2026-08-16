// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Framing;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Moderation;

/// <summary>
/// Server notice that a rule-violation report was cancelled by its reporter (s2c 0xB0).
/// </summary>
public sealed class GameServerRuleViolationCancelMessage : IProtocolMessage
{
    public GameServerRuleViolationCancelMessage(string reporterName)
    {
        ReporterName = reporterName ?? throw new ArgumentNullException(nameof(reporterName));
    }

    public byte Opcode => (byte)GameServerOpcode.RuleViolationCancel;

    public string ReporterName { get; }

    public static IProtocolMessage Read(GameServerOpcode opcode, PacketReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        return new GameServerRuleViolationCancelMessage(reader.ReadString());
    }
}
