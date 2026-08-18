// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Game;

[Action(Id = "LoadItemDatabase", Category = Categories.Game)]
[Group(Name = Groups.General, Order = 1, IsDefault = true)]
[Throws(ErrorCodes.InvalidArgument + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class LoadItemDatabaseAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General)]
    public string DatPath { get; set; } = null!;

    [OutputArgument(Order = 1)]
    public TibiaItemDatabase ItemDatabase { get; set; } = null!;

    protected override void Run(ActionContext context)
    {
        if (string.IsNullOrWhiteSpace(DatPath))
        {
            throw ActionErrors.Create(ErrorCodes.InvalidArgument, ActionErrors.DatPathMissing);
        }

        if (!File.Exists(DatPath))
        {
            throw ActionErrors.Create(ErrorCodes.InvalidArgument, string.Format(ActionErrors.DatFileNotFound, DatPath));
        }

        DatItemDatabase database;
        try
        {
            database = DatItemDatabase.Load(DatPath);
        }
        catch (InvalidDataException exception)
        {
            throw ActionErrors.Create(ErrorCodes.InvalidArgument, exception.Message, exception);
        }
        catch (IOException exception)
        {
            throw ActionErrors.Create(ErrorCodes.InvalidArgument, exception.Message, exception);
        }

        ItemDatabase = new TibiaItemDatabase(database, DatPath);
    }
}
