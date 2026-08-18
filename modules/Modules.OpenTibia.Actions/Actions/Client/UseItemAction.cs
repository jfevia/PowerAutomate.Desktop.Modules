// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Protocol.Geometry;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.ItemActions;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Client;

[Action(Id = "UseItem", Category = Categories.ClientMessages)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Target, Order = 2)]
[Group(Name = Groups.Advanced, Order = 3, IsDefault = true)]
[Throws(ErrorCodes.NotConnected + "Error")]
[Throws(ErrorCodes.InvalidArgument + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class UseItemAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General)]
    public TibiaGameSession GameSession { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.Target)]
    public int X { get; set; }

    [InputArgument(Order = 3, Group = Groups.Target)]
    public int Y { get; set; }

    [InputArgument(Order = 4, Group = Groups.Target)]
    public int Z { get; set; }

    [InputArgument(Order = 5, Group = Groups.Target)]
    public int ItemId { get; set; }

    [InputArgument(Order = 6, Group = Groups.Target)]
    [DefaultValue(0)]
    public int StackPosition { get; set; }

    [InputArgument(Order = 7)]
    [DefaultValue(0)]
    public int ContainerIndex { get; set; }

    protected override void Run(ActionContext context)
    {
        var session = SessionGuards.RequireInGame(GameSession);

        var position = new Position(
            RangeGuard.ToUInt16(X, nameof(X)),
            RangeGuard.ToUInt16(Y, nameof(Y)),
            RangeGuard.ToByte(Z, nameof(Z)));
        var itemId = RangeGuard.ToUInt16(ItemId, nameof(ItemId));
        var stackPosition = RangeGuard.ToByte(StackPosition, nameof(StackPosition));
        var containerIndex = RangeGuard.ToByte(ContainerIndex, nameof(ContainerIndex));

        session.Client.Send(new ClientUseItemMessage(position, itemId, stackPosition, containerIndex));
    }
}
