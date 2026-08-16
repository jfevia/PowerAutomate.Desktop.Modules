// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Moderation;

/// <summary>
/// Registers the GM rule-violation notice message readers.
/// </summary>
public static class ModerationMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(GameServerOpcode.RuleViolationChannel, GameServerRuleViolationChannelMessage.Read);
        registry.Register(GameServerOpcode.RuleViolationRemove, GameServerRuleViolationRemoveMessage.Read);
        registry.Register(GameServerOpcode.RuleViolationCancel, GameServerRuleViolationCancelMessage.Read);

        // Rule-violation-locked notices carry no fields; the opcode alone is the message.
        registry.Register(GameServerOpcode.RuleViolationLock, (opcode, reader) => new PayloadlessMessage(opcode));
    }
}
