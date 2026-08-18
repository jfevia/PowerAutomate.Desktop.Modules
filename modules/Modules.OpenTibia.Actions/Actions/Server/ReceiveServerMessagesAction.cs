// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Client.Streaming;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Server;

/// <summary>
/// Drains a bounded batch from the inbound queue; the wait is always clamped to at most 1 second.
/// </summary>
[Action(Id = "ReceiveServerMessages", Category = Categories.ServerMessages)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.NotConnected + "Error")]
[Throws(ErrorCodes.InvalidArgument + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class ReceiveServerMessagesAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General)]
    public TibiaGameSession GameSession { get; set; } = null!;

    [InputArgument(Order = 2)]
    [DefaultValue(64)]
    public int MaxCount { get; set; } = 64;

    [InputArgument(Order = 3)]
    [DefaultValue(500)]
    public int SliceTimeoutMs { get; set; } = 500;

    [OutputArgument(Order = 1)]
    public DataTable Messages { get; set; } = null!;

    [OutputArgument(Order = 2)]
    public int Count { get; set; }

    [OutputArgument(Order = 3)]
    public bool TimedOut { get; set; }

    protected override void Run(ActionContext context)
    {
        var session = SessionGuards.RequireGame(GameSession);

        if (MaxCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxCount), MaxCount, "Value must be greater than zero.");
        }

        var clampedTimeout = ServerMessageQueue.ClampTimeout(TimeSpan.FromMilliseconds(SliceTimeoutMs));
        var queue = session.Client.Queue;

        var batch = queue == null
            ? Array.Empty<IProtocolMessage>()
            : queue.DequeueBatch(MaxCount, clampedTimeout).ToArray();

        var receivedAt = DateTime.UtcNow;
        var projected = batch.Select(message => ServerMessageProjector.ToServerMessage(message, receivedAt)).ToList();

        Messages = ServerMessageProjector.ToDataTable(projected);
        Count = projected.Count;
        TimedOut = projected.Count == 0;
    }
}
