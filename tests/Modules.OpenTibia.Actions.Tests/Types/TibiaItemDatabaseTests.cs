// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Types;

[TestFixture]
public class TibiaItemDatabaseTests
{
    [Test]
    public void Constructor_WithNullDatabase_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new TibiaItemDatabase(null!, "Tibia.dat"));
    }

    [Test]
    public void Constructor_WithNullPath_Throws()
    {
        var database = DatItemDatabase.Parse(DatFixture.Build(101));

        Assert.Throws<ArgumentNullException>(() => new TibiaItemDatabase(database, null!));
    }
}
