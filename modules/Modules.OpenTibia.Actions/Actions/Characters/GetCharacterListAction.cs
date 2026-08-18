// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Characters;

[Action(Id = "GetCharacterList", Category = Categories.Characters)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.NotConnected + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class GetCharacterListAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General)]
    public TibiaLoginSession LoginSession { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public DataTable Characters { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var session = SessionGuards.RequireLogin(LoginSession);

        Characters = CharacterListProjector.ToDataTable(session.Result.Characters);
    }
}
