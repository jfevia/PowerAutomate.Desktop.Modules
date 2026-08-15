// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Data;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.HTML.Actions.Tests.Actions;

[TestFixture]
public class ConvertDataTableToHtmlTableActionTests
{
    [Test]
    public void Execute_WithRowsAndColumns_CreatesTableMarkup()
    {
        var table = new DataTable();
        table.Columns.Add("Name");
        table.Columns.Add("Count", typeof(int));
        table.Rows.Add("one", 1);
        table.Rows.Add("two", 2);
        var action = new ConvertDataTableToHtmlTableAction(new HtmlActionsContext(new HtmlMarkupService())) { DataTable = table };

        action.Execute(new ActionContext());

        Assert.That(action.HtmlTable, Is.EqualTo("<table><tr><th>Name</th><th>Count</th></tr><tr><td>one</td><td>1</td></tr><tr><td>two</td><td>2</td></tr></table>"));
    }

    [Test]
    public void Execute_WithEmptyTable_CreatesEmptyTableMarkup()
    {
        var action = new ConvertDataTableToHtmlTableAction(new HtmlActionsContext(new HtmlMarkupService())) { DataTable = new DataTable() };

        action.Execute(new ActionContext());

        Assert.That(action.HtmlTable, Is.EqualTo("<table><tr></tr></table>"));
    }

    [Test]
    public void Execute_WhenDataTableIsMissing_ThrowsUnknownError()
    {
        var action = new ConvertDataTableToHtmlTableAction(new HtmlActionsContext(new HtmlMarkupService()));

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new ConvertDataTableToHtmlTableAction();

        Assert.That(action.HtmlTable, Is.Null);
    }
}
