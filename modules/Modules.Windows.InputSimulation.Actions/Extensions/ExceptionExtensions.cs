// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Extensions;

internal static class ExceptionExtensions
{
    /// <summary>
    /// Maps an internal failure onto the error code the flow author can branch on.
    /// </summary>
    public static ActionException ToActionException(this Exception value)
    {
        return value switch
        {
            ActionException actionException => actionException,
            AccessDeniedException => new ActionException(ErrorCodes.AccessDenied, value.Message, value),
            WindowNotFoundException => new ActionException(ErrorCodes.WindowNotFound, value.Message, value),
            ControlNotFoundException => new ActionException(ErrorCodes.ControlNotFound, value.Message, value),
            MessageDeliveryException => new ActionException(ErrorCodes.MessageDelivery, value.Message, value),
            _ => new ActionException(ErrorCodes.Unknown, value.Message, value)
        };
    }
}
