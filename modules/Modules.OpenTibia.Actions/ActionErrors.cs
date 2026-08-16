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
