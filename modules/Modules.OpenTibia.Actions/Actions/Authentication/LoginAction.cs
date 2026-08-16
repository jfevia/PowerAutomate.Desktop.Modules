// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Authentication;

[Action(Id = "Login", Category = Categories.Authentication)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.ConnectionFailed + "Error")]
[Throws(ErrorCodes.AuthenticationFailed + "Error")]
[Throws(ErrorCodes.Timeout + "Error")]
[Throws(ErrorCodes.ProtocolError + "Error")]
[Throws(ErrorCodes.InvalidArgument + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class LoginAction : OpenTibiaActionBase
{
    private readonly Func<ISocketTransport> _transportFactory;
    private readonly Func<uint[]> _keyFactory;

    public LoginAction() : this(() => new SocketTransport(), XteaKeyGenerator.Generate)
    {
    }

    public LoginAction(Func<ISocketTransport> transportFactory, Func<uint[]> keyFactory)
    {
        _transportFactory = transportFactory ?? throw new ArgumentNullException(nameof(transportFactory));
        _keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
    }

    [InputArgument(Order = 1, Group = Groups.General)]
    public string Host { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.General)]
    [DefaultValue(7171)]
    public int Port { get; set; } = 7171;

    [InputArgument(Order = 3, Group = Groups.General)]
    public string AccountName { get; set; } = null!;

    [InputArgument(Order = 4, Group = Groups.General)]
    public string Password { get; set; } = null!;

    [InputArgument(Order = 5)]
    [DefaultValue(15000)]
    public int TimeoutMs { get; set; } = 15000;

    [OutputArgument(Order = 1)]
    public TibiaLoginSession LoginSession { get; set; } = null!;

    [OutputArgument(Order = 2)]
    public string Motd { get; set; } = null!;

    [OutputArgument(Order = 3)]
    public DataTable Characters { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var options = new LoginOptions(Host, Port, AccountName, Password);
        var transport = _transportFactory();
        var client = new TibiaLoginClient(transport, _keyFactory);

        LoginResult result;
        try
        {
            result = client.Authenticate(options, TimeSpan.FromMilliseconds(TimeoutMs));
        }
        catch (LoginRejectedException exception)
        {
            throw ActionErrors.Create(ErrorCodes.AuthenticationFailed, exception.Message, exception);
        }
        catch (SocketException exception)
        {
            throw ActionErrors.Create(ErrorCodes.ConnectionFailed, exception.Message, exception);
        }

        LoginSession = new TibiaLoginSession(transport, client, Host, Port, AccountName, Password, result);
        Motd = result.Motd;
        Characters = CharacterListProjector.ToDataTable(result.Characters);
    }
}
