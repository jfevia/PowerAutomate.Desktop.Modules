// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Server;

/// <summary>
/// A single-shot, non-blocking peek meant to be polled by a wait action loop.
/// </summary>
[WaitAction(Id = "HasServerMessage", Category = Categories.ServerMessages, ResultPropertyName = nameof(HasMessage))]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class HasServerMessageAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General, Required = false)]
    public TibiaGameSession GameSession { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public bool HasMessage { get; set; }

    protected override void Run(ActionContext context)
    {
        HasMessage = GameSession?.Client.Queue != null && GameSession.Client.Queue.Depth > 0;
    }
}
