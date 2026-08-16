// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;

namespace PowerAutomate.Desktop.OpenTibia.Client.Handshake;

/// <summary>
/// Connection settings for entering the game world.
/// </summary>
public sealed class GameOptions
{
    public GameOptions(string host, int port, string accountName, string characterName, string password)
    {
        Host = host ?? throw new ArgumentNullException(nameof(host));
        AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
        CharacterName = characterName ?? throw new ArgumentNullException(nameof(characterName));
        Password = password ?? throw new ArgumentNullException(nameof(password));

        if (port <= 0 || port > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port));
        }

        Port = port;
    }

    public string Host { get; }

    public int Port { get; }

    public string AccountName { get; }

    public string CharacterName { get; }

    public string Password { get; }

    public ushort Version { get; set; } = 860;

    public ushort Os { get; set; } = 2;

    public bool IsGamemaster { get; set; }

    /// <summary>
    /// Ring buffer size; overflow discards the oldest message and increments the drop counter.
    /// </summary>
    public int QueueCapacity { get; set; } = 4096;
}

/// <summary>
/// Raised when the game server refuses the character.
/// </summary>
public class GameLoginRejectedException : Exception
{
    public GameLoginRejectedException(string message) : base(message)
    {
    }
}
