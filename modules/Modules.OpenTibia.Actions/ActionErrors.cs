// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using PowerAutomate.Desktop.OpenTibia.Protocol;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions;

/// <summary>
/// Translates library exceptions into the error codes a flow author can branch on.
/// </summary>
public static class ActionErrors
{
    /// <summary>
    /// Shown when Load item database is called without a path.
    /// </summary>
    public const string DatPathMissing = "A path to the client's Tibia.dat is required.";

    /// <summary>
    /// Shown when the configured Tibia.dat does not exist.
    /// </summary>
    public const string DatFileNotFound = "Tibia.dat was not found at '{0}'.";

    /// <summary>
    /// Shown when Enter game is called without item classification, which the map decoder needs.
    /// </summary>
    public const string ItemDatabaseMissing =
        "An item database is required. Run the Load item database action against the client's "
        + "Tibia.dat and pass its output here. Without it every stackable, fluid and splash item "
        + "decodes one byte short and the map description desynchronizes.";

    public static ActionException Create(string code, string message)
    {
        return new ActionException(code + "Error", message);
    }

    public static ActionException Create(string code, string message, Exception inner)
    {
        return new ActionException(code + "Error", message, inner);
    }

    /// <summary>
    /// Maps an arbitrary failure onto the closest error code.
    /// </summary>
    public static ActionException Translate(Exception exception)
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        if (exception is ActionException actionException)
        {
            return actionException;
        }

        if (exception is ProtocolException)
        {
            return Create(ErrorCodes.ProtocolError, exception.Message, exception);
        }

        if (exception is TimeoutException)
        {
            return Create(ErrorCodes.Timeout, exception.Message, exception);
        }

        if (exception is ArgumentException)
        {
            return Create(ErrorCodes.InvalidArgument, exception.Message, exception);
        }

        return Create(ErrorCodes.Unknown, exception.Message, exception);
    }
}
