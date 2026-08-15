// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Actions;
using PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.InputSimulation.Actions.Tests.Actions;

[TestFixture]
public class FindWindowActionTests
{
    [SetUp]
    public void SetUp() => _test = new ActionHarness();

    private ActionHarness _test = null!;

    [Test]
    public void Execute_WhenWindowFound_SetsWindowOutput()
    {
        _test.WindowService.FoundWindow = ActionHarness.Handle(55);
        _test.WindowService.ClassNameResult = "Notepad";

        var action = new FindWindowAction(_test.Context) { Title = "Untitled" };
        action.Execute(new ActionContext());

        Assert.That(action.Window, Is.Not.Null);
        Assert.That(action.Window.Handle, Is.EqualTo(55));
        Assert.That(action.Window.ClassName, Is.EqualTo("Notepad"));
    }

    [Test]
    public void Execute_WhenWindowMissing_ThrowsWindowNotFoundError()
    {
        _test.WindowService.FoundWindow = IntPtr.Zero;
        _test.WindowService.CriteriaDescription = "title 'Nope'";

        var action = new FindWindowAction(_test.Context) { Title = "Nope" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()));
        Assert.That(exception.Name, Is.EqualTo(ErrorCodes.WindowNotFound));
        Assert.That(exception.Message, Does.Contain("Nope"));
    }

    [Test]
    public void Execute_WithEveryCriterion_StillResolvesWindow()
    {
        _test.WindowService.FoundWindow = ActionHarness.Handle(9);

        var action = new FindWindowAction(_test.Context)
        {
            Title = "Title",
            ClassName = "Class",
            ProcessId = 1234,
            TimeoutMilliseconds = 250
        };
        action.Execute(new ActionContext());

        Assert.That(action.Window.Handle, Is.EqualTo(9));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new FindWindowAction();

        Assert.That(action.MatchMode, Is.EqualTo(Enums.TextMatchMode.Contains));
        Assert.That(action.TimeoutMilliseconds, Is.Zero);
    }
}
