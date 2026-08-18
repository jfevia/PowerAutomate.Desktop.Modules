// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;

/// <summary>
/// Central null/state guards shared by every action that needs a live session.
/// </summary>
public static class SessionGuards
{
    /// <summary>
    /// Requires a login session whose socket is still open.
    /// </summary>
    public static TibiaLoginSession RequireLogin(TibiaLoginSession? session)
    {
        if (session == null || !session.Transport.IsConnected)
        {
            throw ActionErrors.Create(ErrorCodes.NotConnected, "No active login session. Run Login first.");
        }

        return session;
    }

    /// <summary>
    /// Requires a game session to exist, regardless of its current connection state.
    /// </summary>
    public static TibiaGameSession RequireGame(TibiaGameSession? session)
    {
        if (session == null)
        {
            throw ActionErrors.Create(ErrorCodes.NotConnected, "No active game session. Run Enter game first.");
        }

        return session;
    }

    /// <summary>
    /// Requires a game session that is actively in game and not faulted.
    /// </summary>
    public static TibiaGameSession RequireInGame(TibiaGameSession? session)
    {
        var required = RequireGame(session);

        if (required.Client.FaultReason != null || required.Client.State != ConnectionState.InGame)
        {
            throw ActionErrors.Create(ErrorCodes.NotConnected, $"The game session is not connected (state: {required.State}).");
        }

        return required;
    }
}
