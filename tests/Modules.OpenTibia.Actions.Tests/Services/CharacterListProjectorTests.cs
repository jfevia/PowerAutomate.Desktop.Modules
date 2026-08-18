// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using System.Data;
using System.Linq;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Login;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Services;

[TestFixture]
public class CharacterListProjectorTests
{
    private static CharacterListEntry Entry()
    {
        return new CharacterListEntry("Rook", "Antica", 0x0100007F, 7172);
    }

    [Test]
    public void ToDataTable_WithNullSequence_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CharacterListProjector.ToDataTable(null!));
    }

    [Test]
    public void ToDataTable_WithNoCharacters_ReturnsAnEmptyTable()
    {
        Assert.That(CharacterListProjector.ToDataTable(Array.Empty<CharacterListEntry>()).Rows, Is.Empty);
    }

    [Test]
    public void ToDataTable_BuildsTheDocumentedColumns()
    {
        var table = CharacterListProjector.ToDataTable(new[] { Entry() });

        Assert.Multiple(() =>
        {
            Assert.That(table.Columns.Cast<DataColumn>().Select(column => column.ColumnName),
                Is.EqualTo(new[] { "Name", "World", "Host", "Port" }));
            Assert.That(table.Rows, Has.Count.EqualTo(1));
            Assert.That(table.Rows[0][CharacterListProjector.NameColumn], Is.EqualTo("Rook"));
            Assert.That(table.Rows[0][CharacterListProjector.WorldColumn], Is.EqualTo("Antica"));
            Assert.That(table.Rows[0][CharacterListProjector.HostColumn], Is.EqualTo("127.0.0.1"));
            Assert.That(table.Rows[0][CharacterListProjector.PortColumn], Is.EqualTo(7172));
        });
    }

    [Test]
    public void ToTibiaCharacter_WithNullEntry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CharacterListProjector.ToTibiaCharacter(null!));
    }

    [Test]
    public void ToTibiaCharacter_ProjectsEveryField()
    {
        var character = CharacterListProjector.ToTibiaCharacter(Entry());

        Assert.Multiple(() =>
        {
            Assert.That(character.Name, Is.EqualTo("Rook"));
            Assert.That(character.World, Is.EqualTo("Antica"));
            Assert.That(character.Host, Is.EqualTo("127.0.0.1"));
            Assert.That(character.Port, Is.EqualTo(7172));
        });
    }
}
