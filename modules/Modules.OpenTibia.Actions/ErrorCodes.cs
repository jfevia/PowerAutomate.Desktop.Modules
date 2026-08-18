// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions;

/// <summary>
/// Error codes surfaced to desktop flows through the error handling dialog.
/// </summary>
public static class ErrorCodes
{
    public const string ConnectionFailed = nameof(ConnectionFailed);
    public const string AuthenticationFailed = nameof(AuthenticationFailed);
    public const string CharacterNotFound = nameof(CharacterNotFound);
    public const string NotConnected = nameof(NotConnected);
    public const string AlreadyConnected = nameof(AlreadyConnected);
    public const string ProtocolError = nameof(ProtocolError);
    public const string Timeout = nameof(Timeout);
    public const string InvalidArgument = nameof(InvalidArgument);
    public const string Unknown = nameof(Unknown);
}
