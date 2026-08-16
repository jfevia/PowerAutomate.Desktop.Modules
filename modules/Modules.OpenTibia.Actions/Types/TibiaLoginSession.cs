// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Enums;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

/// <summary>
/// A live login-server connection carried between actions by a flow variable.
/// </summary>
[Type(FriendlyName = nameof(TibiaLoginSession) + "_FriendlyName",
      FriendlyNamePlural = nameof(TibiaLoginSession) + "_FriendlyNamePlural",
      DefaultPropertyVisibility = Visibility.Visible)]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Type")]
public sealed class TibiaLoginSession
{
    public TibiaLoginSession(
        ISocketTransport transport,
        TibiaLoginClient client,
        string host,
        int port,
        string accountName,
        string password,
        LoginResult result)
    {
        Transport = transport ?? throw new ArgumentNullException(nameof(transport));
        Client = client ?? throw new ArgumentNullException(nameof(client));
        Host = host ?? throw new ArgumentNullException(nameof(host));
        AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
        Password = password ?? throw new ArgumentNullException(nameof(password));
        Result = result ?? throw new ArgumentNullException(nameof(result));
        Port = port;
    }

    /// <summary>
    /// The socket used for the login exchange; closed by the Logout action.
    /// </summary>
    [PropertyIgnore]
    public ISocketTransport Transport { get; }

    [PropertyIgnore]
    public TibiaLoginClient Client { get; }

    /// <summary>
    /// Carried forward so Enter game can authenticate against the game server.
    /// </summary>
    [PropertyIgnore]
    public string Password { get; }

    [PropertyIgnore]
    public LoginResult Result { get; }

    [Property]
    public string Host { get; }

    [Property]
    public int Port { get; }

    [Property]
    public string AccountName { get; }

    [Property]
    public string Motd => Result.Motd;

    [Property]
    public int PremiumDays => Result.PremiumDays;

    public override string ToString()
    {
        return $"{AccountName} @ {Host}:{Port}";
    }
}
