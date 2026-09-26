namespace PowerAutomate.Desktop.Modules.RabbitMQ.Actions;

/// <summary>
///     Identifies RabbitMQ failures handled by desktop flows.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    ///     Reports a failed broker request or connection.
    /// </summary>
    public const string Broker = "BrokerError";

    /// <summary>
    ///     Reports an attempt to use a closed connection.
    /// </summary>
    public const string ClosedConnection = "ClosedConnectionError";

    /// <summary>
    ///     Reports an invalid URI, message, or queue input.
    /// </summary>
    public const string InvalidArgument = "InvalidArgumentError";

    /// <summary>
    ///     Reports an attempt to acknowledge or reject twice.
    /// </summary>
    public const string MessageSettled = "MessageSettledError";
}
