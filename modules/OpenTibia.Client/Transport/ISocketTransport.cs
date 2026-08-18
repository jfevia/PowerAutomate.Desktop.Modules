// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.OpenTibia.Client.Transport;

/// <summary>
/// Lifecycle of a connection, checked by every action before it runs.
/// </summary>
public enum ConnectionState
{
    Disconnected = 0,
    Connecting = 1,
    Authenticating = 2,
    CharacterList = 3,
    EnteringGame = 4,
    InGame = 5,
    Disconnecting = 6,
    Faulted = 7
}

/// <summary>
/// The socket operations the client needs, isolated so the protocol logic can be tested without a network.
/// </summary>
public interface ISocketTransport : IDisposable
{
    bool IsConnected { get; }

    void Connect(string host, int port, TimeSpan timeout);

    /// <summary>
    /// Returns zero when the peer closed the connection, or a negative value when the read timed out.
    /// </summary>
    int Read(byte[] buffer, int offset, int count);

    void Write(byte[] data);

    void Close();
}
