// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Collections.Generic;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.HTML.Actions.Tests.Actions;

[TestFixture]
public class ConvertListToHtmlListActionTests
{
    [Test]
    public void Execute_WithUnorderedList_CreatesUnorderedMarkup()
    {
        var action = new ConvertListToHtmlListAction(new HtmlActionsContext(new HtmlMarkupService())) { List = new List<object> { "one", 2 }, IsOrdered = false };

        action.Execute(new ActionContext());

        Assert.That(action.HtmlList, Is.EqualTo("<ul><li>one</li><li>2</li></ul>"));
    }

    [Test]
    public void Execute_WithOrderedEmptyList_CreatesOrderedMarkup()
    {
        var action = new ConvertListToHtmlListAction(new HtmlActionsContext(new HtmlMarkupService())) { List = new List<object>(), IsOrdered = true };

        action.Execute(new ActionContext());

        Assert.That(action.HtmlList, Is.EqualTo("<ol></ol>"));
    }

    [Test]
    public void Execute_WhenListIsMissing_ThrowsUnknownError()
    {
        var action = new ConvertListToHtmlListAction(new HtmlActionsContext(new HtmlMarkupService())) { IsOrdered = false };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new ConvertListToHtmlListAction();

        Assert.That(action.IsOrdered, Is.False);
    }
}
