// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Characters;

[Action(Id = "SelectCharacter", Category = Categories.Characters)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.NotConnected + "Error")]
[Throws(ErrorCodes.CharacterNotFound + "Error")]
[Throws(ErrorCodes.InvalidArgument + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class SelectCharacterAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General)]
    public TibiaLoginSession LoginSession { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.General)]
    public string CharacterName { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public TibiaCharacter Character { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        var session = SessionGuards.RequireLogin(LoginSession);

        if (string.IsNullOrEmpty(CharacterName))
        {
            throw new ArgumentException("Character name must not be empty.", nameof(CharacterName));
        }

        var entry = session.Result.Characters
            .FirstOrDefault(candidate => string.Equals(candidate.Name, CharacterName, StringComparison.OrdinalIgnoreCase));

        if (entry == null)
        {
            var available = string.Join(", ", session.Result.Characters.Select(candidate => candidate.Name));
            throw ActionErrors.Create(
                ErrorCodes.CharacterNotFound,
                $"Character '{CharacterName}' was not found. Available characters: {available}.");
        }

        Character = CharacterListProjector.ToTibiaCharacter(entry);
    }
}
