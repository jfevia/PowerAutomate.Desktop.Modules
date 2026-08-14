// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Exceptions;

/// <summary>
/// Raised when a message cannot be delivered, or the target stops pumping messages before answering.
/// </summary>
public sealed class MessageDeliveryException : Exception
{
    public MessageDeliveryException(string message) : base(message)
    {
    }
}
