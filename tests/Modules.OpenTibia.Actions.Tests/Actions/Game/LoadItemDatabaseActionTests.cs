// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.IO;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Game;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Tests.Actions.Game;

[TestFixture]
public class LoadItemDatabaseActionTests
{
    private string _path = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".dat");
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }

    [Test]
    public void Execute_WithValidDat_PopulatesItemDatabase()
    {
        File.WriteAllBytes(_path, DatFixture.Build(110, 105, 107));
        var action = new LoadItemDatabaseAction { DatPath = _path };

        action.Execute(new ActionContext());

        Assert.Multiple(() =>
        {
            Assert.That(action.ItemDatabase.ItemCount, Is.EqualTo(11));
            Assert.That(action.ItemDatabase.StackableCount, Is.EqualTo(2));
            Assert.That(action.ItemDatabase.FluidContainerCount, Is.Zero);
            Assert.That(action.ItemDatabase.SplashCount, Is.Zero);
            Assert.That(action.ItemDatabase.Path, Is.EqualTo(_path));
            Assert.That(action.ItemDatabase.Signature, Is.EqualTo("0x4C28B721"));
            Assert.That(action.ItemDatabase.ToString(), Does.Contain("11 items"));
        });
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public void Execute_WithBlankPath_ThrowsInvalidArgument(string? path)
    {
        var action = new LoadItemDatabaseAction { DatPath = path! };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
    }

    [Test]
    public void Execute_WithMissingFile_ThrowsInvalidArgument()
    {
        var action = new LoadItemDatabaseAction { DatPath = _path };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Message, Does.Contain("was not found"));
    }

    [Test]
    public void Execute_WithTruncatedDat_ThrowsInvalidArgument()
    {
        File.WriteAllBytes(_path, new byte[] { 0x21, 0xB7, 0x28, 0x4C, 0x00 });
        var action = new LoadItemDatabaseAction { DatPath = _path };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
    }

    [Test]
    public void Execute_WithTooFewThings_ThrowsInvalidArgument()
    {
        // Declares 10 things, fewer than the first item id of 100.
        File.WriteAllBytes(_path, new byte[] { 0x21, 0xB7, 0x28, 0x4C, 0x0A, 0x00, 0, 0, 0, 0, 0, 0 });
        var action = new LoadItemDatabaseAction { DatPath = _path };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Message, Does.Contain("fewer than the first item id"));
    }

    [Test]
    public void Execute_WhenFileIsLocked_ThrowsInvalidArgument()
    {
        File.WriteAllBytes(_path, DatFixture.Build(101));
        var action = new LoadItemDatabaseAction { DatPath = _path };

        using (new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

            Assert.That(exception.Name, Is.EqualTo("InvalidArgumentError"));
        }
    }

    [Test]
    public void Execute_WhenPathIsADirectory_ThrowsInvalidArgument()
    {
        var action = new LoadItemDatabaseAction { DatPath = Path.GetTempPath() };

        Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
    }
}
