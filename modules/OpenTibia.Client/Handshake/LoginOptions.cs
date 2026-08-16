// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

namespace PowerAutomate.Desktop.OpenTibia.Client.Handshake;

/// <summary>
/// Everything the login server told us about an account.
/// </summary>
public sealed class LoginResult
{
    public LoginResult(IReadOnlyList<CharacterListEntry> characters, string motd, ushort premiumDays)
    {
        Characters = characters ?? throw new ArgumentNullException(nameof(characters));
        Motd = motd ?? throw new ArgumentNullException(nameof(motd));
        PremiumDays = premiumDays;
    }

    public IReadOnlyList<CharacterListEntry> Characters { get; }

    public string Motd { get; }

    public ushort PremiumDays { get; }
}

/// <summary>
/// Connection settings for the login-server exchange.
/// </summary>
public sealed class LoginOptions
{
    public LoginOptions(string host, int port, string accountName, string password)
    {
        Host = host ?? throw new ArgumentNullException(nameof(host));
        AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
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

    public string Password { get; }

    /// <summary>
    /// Client build reported to the server; 8.60 is 860.
    /// </summary>
    public ushort Version { get; set; } = 860;

    /// <summary>
    /// Operating system identifier; 2 is the Windows value the classic client sends.
    /// </summary>
    public ushort Os { get; set; } = 2;
}
