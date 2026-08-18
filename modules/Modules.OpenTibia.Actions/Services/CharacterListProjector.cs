// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;

/// <summary>
/// Converts the login server's character list into the shapes a desktop flow can consume.
/// </summary>
public static class CharacterListProjector
{
    public const string NameColumn = "Name";

    public const string WorldColumn = "World";

    public const string HostColumn = "Host";

    public const string PortColumn = "Port";

    public static DataTable ToDataTable(IEnumerable<CharacterListEntry> characters)
    {
        if (characters == null)
        {
            throw new ArgumentNullException(nameof(characters));
        }

        var table = new DataTable("Characters");
        table.Columns.Add(NameColumn, typeof(string));
        table.Columns.Add(WorldColumn, typeof(string));
        table.Columns.Add(HostColumn, typeof(string));
        table.Columns.Add(PortColumn, typeof(int));

        foreach (var character in characters)
        {
            table.Rows.Add(character.Name, character.World, character.HostName, (int)character.Port);
        }

        return table;
    }

    public static TibiaCharacter ToTibiaCharacter(CharacterListEntry entry)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        return new TibiaCharacter(entry.Name, entry.World, entry.HostName, entry.Port);
    }
}
