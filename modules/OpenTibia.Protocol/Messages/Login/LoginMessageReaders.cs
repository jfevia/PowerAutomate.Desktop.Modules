// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

/// <summary>
/// Registers the game-server login opcode readers on a <see cref="GameServerMessageRegistry"/>.
/// </summary>
public static class LoginMessageReaders
{
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        registry.Register(GameServerOpcode.Challenge, GameServerChallengeMessage.Read);
        registry.Register(GameServerOpcode.LoginError, GameServerLoginErrorMessage.Read);
        registry.Register(GameServerOpcode.LoginAdvice, GameServerLoginAdviceMessage.Read);
        registry.Register(GameServerOpcode.LoginWait, GameServerLoginWaitMessage.Read);
        registry.Register(GameServerOpcode.LoginOrPendingState, GameServerLoginOrPendingStateMessage.Read);
    }
}
