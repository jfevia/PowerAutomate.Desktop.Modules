// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Client;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Game;

[Action(Id = "EnterGame", Category = Categories.Game)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.NotConnected + "Error")]
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
public class EnterGameAction : OpenTibiaActionBase
{
    private readonly Func<ISocketTransport> _transportFactory;
    private readonly Func<uint[]> _keyFactory;

    public EnterGameAction() : this(() => new SocketTransport(), XteaKeyGenerator.Generate)
    {
    }

    internal EnterGameAction(Func<ISocketTransport> transportFactory, Func<uint[]> keyFactory)
    {
        _transportFactory = transportFactory ?? throw new ArgumentNullException(nameof(transportFactory));
        _keyFactory = keyFactory ?? throw new ArgumentNullException(nameof(keyFactory));
    }

    [InputArgument(Order = 1, Group = Groups.General)]
    public TibiaLoginSession LoginSession { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.General)]
    public TibiaCharacter Character { get; set; } = null!;

    [InputArgument(Order = 3, Group = Groups.General)]
    public TibiaItemDatabase ItemDatabase { get; set; } = null!;

    [InputArgument(Order = 4)]
    [DefaultValue(4096)]
    public int QueueCapacity { get; set; } = 4096;

    [InputArgument(Order = 5)]
    [DefaultValue(15000)]
    public int TimeoutMs { get; set; } = 15000;

    [OutputArgument(Order = 1)]
    public TibiaGameSession GameSession { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var session = SessionGuards.RequireLogin(LoginSession);

        if (Character == null)
        {
            throw new ArgumentNullException(nameof(Character));
        }

        // Without item classification the map decoder desynchronizes and reports a bogus opcode.
        if (ItemDatabase == null)
        {
            throw ActionErrors.Create(ErrorCodes.InvalidArgument, ActionErrors.ItemDatabaseMissing);
        }

        var options = new GameOptions(Character.Host, Character.Port, session.AccountName, Character.Name, session.Password)
        {
            QueueCapacity = QueueCapacity
        };

        var transport = _transportFactory();
        var floors = new MapFloorTracker();
        var registry = GameServerRegistryFactory.CreateDefault(ItemDatabase.Provider, floors);
        var client = new TibiaGameClient(transport, registry, _keyFactory, floors);

        try
        {
            client.EnterGame(options, TimeSpan.FromMilliseconds(TimeoutMs));
        }
        catch (GameLoginRejectedException exception)
        {
            throw ActionErrors.Create(ErrorCodes.AuthenticationFailed, exception.Message, exception);
        }
        catch (SocketException exception)
        {
            throw ActionErrors.Create(ErrorCodes.ConnectionFailed, exception.Message, exception);
        }

        GameSession = new TibiaGameSession(client, Character.Name, Character.World);
    }
}
