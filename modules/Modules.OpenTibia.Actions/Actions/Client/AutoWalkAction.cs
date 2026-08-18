// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Enums;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using ClientAutoWalkMessage = PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement.ClientAutoWalkMessage;
using ClientTurnMessage = PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement.ClientTurnMessage;
using ClientWalkMessage = PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Movement.ClientWalkMessage;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Client;

[Action(Id = "AutoWalk", Category = Categories.ClientMessages)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.NotConnected + "Error")]
[Throws(ErrorCodes.InvalidArgument + "Error")]
[Throws(ErrorCodes.ProtocolError + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class AutoWalkAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General)]
    public TibiaGameSession GameSession { get; set; } = null!;

    /// <summary>
    /// A comma separated path such as n,n,e,s,s,w; full names like North are also accepted.
    /// </summary>
    /// <remarks>
    /// A string rather than a list of Direction because the designer cannot express an enum literal
    /// inside a list: a list argument is not flagged as an enum, so each element is validated as a
    /// variable and rejected.
    /// </remarks>
    [InputArgument(Order = 2, Group = Groups.General)]
    public string Directions { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var session = SessionGuards.RequireInGame(GameSession);

        if (Directions == null)
        {
            throw new ArgumentNullException(nameof(Directions));
        }

        session.Client.Send(new ClientAutoWalkMessage(DirectionPath.Parse(Directions).ToProtocol()));
    }
}
