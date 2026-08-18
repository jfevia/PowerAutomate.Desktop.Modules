// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Client.Transport;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Game;

/// <summary>
/// A non-throwing state peek: a missing or faulted session simply evaluates to false.
/// </summary>
[ConditionAction(Id = "IsInGame", Category = Categories.Game, ResultPropertyName = nameof(IsInGame))]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class IsInGameAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General, Required = false)]
    public TibiaGameSession GameSession { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public bool IsInGame { get; set; }

    protected override void Run(ActionContext context)
    {
        IsInGame = GameSession != null
                   && GameSession.Client.FaultReason == null
                   && GameSession.Client.State == ConnectionState.InGame;
    }
}
