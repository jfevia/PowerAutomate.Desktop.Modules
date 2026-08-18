// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.ComponentModel;
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

[Action(Id = "Move", Category = Categories.ClientMessages)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.NotConnected + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class MoveAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General)]
    public TibiaGameSession GameSession { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.General)]
    [DefaultValue(Direction.North)]
    public Direction Direction { get; set; } = Direction.North;

    protected override void Run(ActionContext context)
    {
        var session = SessionGuards.RequireInGame(GameSession);

        session.Client.Send(new ClientWalkMessage(Direction.ToProtocol()));
    }
}
