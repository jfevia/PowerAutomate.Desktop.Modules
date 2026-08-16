// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Server;

[Action(Id = "GetStreamStatistics", Category = Categories.ServerMessages)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.NotConnected + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetStreamStatisticsAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General)]
    public TibiaGameSession GameSession { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public int Depth { get; set; }

    [OutputArgument(Order = 2)]
    public int Capacity { get; set; }

    [OutputArgument(Order = 3)]
    public long Enqueued { get; set; }

    [OutputArgument(Order = 4)]
    public long Dequeued { get; set; }

    [OutputArgument(Order = 5)]
    public long Dropped { get; set; }

    [OutputArgument(Order = 6)]
    public long Filtered { get; set; }

    [OutputArgument(Order = 7)]
    public int MaxDepthSeen { get; set; }

    protected override void Run(ActionContext context)
    {
        var session = SessionGuards.RequireGame(GameSession);

        var statistics = session.Client.Queue?.GetStatistics() ?? default;

        Depth = statistics.Depth;
        Capacity = statistics.Capacity;
        Enqueued = statistics.Enqueued;
        Dequeued = statistics.Dequeued;
        Dropped = statistics.Dropped;
        Filtered = statistics.Filtered;
        MaxDepthSeen = statistics.MaxDepthSeen;
    }
}
