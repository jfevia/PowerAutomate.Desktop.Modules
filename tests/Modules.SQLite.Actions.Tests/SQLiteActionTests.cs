using System;
using System.Data;
using System.IO;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions.Tests;

[TestFixture]
public class SQLiteActionTests
{
    private string path = null!;

    [SetUp]
    public void SetUp() => path = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Guid.NewGuid():N}.db");

    [TearDown]
    public void TearDown() => File.Delete(path);

    [Test]
    public void Execute_CreatesDatabaseOnlyWhenRequested()
    {
        var missing = new ExecuteAction { DatabasePath = path, Sql = "CREATE TABLE records (value INTEGER)" };
        Assert.That(Assert.Throws<ActionException>(() => missing.Execute(new ActionContext()))!.Name,
            Is.EqualTo(ErrorCodes.Database));
        Assert.That(File.Exists(path), Is.False);

        missing.CreateIfMissing = true;
        missing.Execute(new ActionContext());
        Assert.That(File.Exists(path), Is.True);
    }

    [Test]
    public void Execute_BindsValuesAndReturnsAffectedRows()
    {
        CreateDatabase();
        var action = new ExecuteAction
        {
            DatabasePath = path,
            Sql = "INSERT INTO records (value) VALUES ($value)",
            Parameters = Parameters(("$value", 42))
        };

        action.Execute(new ActionContext());

        Assert.That(action.AffectedRows, Is.EqualTo(1));
        var query = new QueryAction { DatabasePath = path, Sql = "SELECT value FROM records" };
        query.Execute(new ActionContext());
        Assert.That(Convert.ToInt64(query.Result.Rows[0]["value"]), Is.EqualTo(42));
    }

    [Test]
    public void Query_BindsNullAndReturnsTable()
    {
        CreateDatabase();
        var action = new QueryAction
        {
            DatabasePath = path,
            Sql = "SELECT $value AS value",
            Parameters = Parameters(("$value", DBNull.Value))
        };

        action.Execute(new ActionContext());

        Assert.That(action.Result.Rows.Count, Is.EqualTo(1));
        Assert.That(action.Result.Rows[0].IsNull("value"), Is.True);
    }

    [Test]
    public void Query_WhenDatabaseIsMissing_ReportsDatabaseError()
    {
        var action = new QueryAction { DatabasePath = path, Sql = "SELECT 1" };

        Assert.That(Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!.Name,
            Is.EqualTo(ErrorCodes.Database));
        Assert.That(File.Exists(path), Is.False);
    }

    [TestCase("")]
    [TestCase("relative.db")]
    [TestCase("C:relative.db")]
    [TestCase("\\root-relative.db")]
    public void Execute_WithInvalidPath_ReportsInvalidArgument(string databasePath)
    {
        var action = new ExecuteAction { DatabasePath = databasePath, Sql = "SELECT 1" };

        Assert.That(Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!.Name,
            Is.EqualTo(ErrorCodes.InvalidArgument));
    }

    [Test]
    public void Query_WithBlankSql_ReportsInvalidArgument()
    {
        CreateDatabase();
        var action = new QueryAction { DatabasePath = path, Sql = " " };

        Assert.That(Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!.Name,
            Is.EqualTo(ErrorCodes.InvalidArgument));
    }

    [Test]
    public void Execute_WithBlankSql_DoesNotCreateDatabase()
    {
        var action = new ExecuteAction { DatabasePath = path, CreateIfMissing = true, Sql = " " };

        Assert.That(Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!.Name,
            Is.EqualTo(ErrorCodes.InvalidArgument));
        Assert.That(File.Exists(path), Is.False);
    }

    [Test]
    public void Execute_WithInvalidParameters_DoesNotCreateDatabase()
    {
        var action = new ExecuteAction
        {
            DatabasePath = path,
            CreateIfMissing = true,
            Sql = "SELECT 1",
            Parameters = new DataTable()
        };

        Assert.That(Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!.Name,
            Is.EqualTo(ErrorCodes.InvalidArgument));
        Assert.That(File.Exists(path), Is.False);
    }

    [Test]
    public void Execute_WithInvalidSql_ReportsDatabaseError()
    {
        CreateDatabase();
        var action = new ExecuteAction { DatabasePath = path, Sql = "NOT VALID SQL" };

        Assert.That(Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!.Name,
            Is.EqualTo(ErrorCodes.Database));
    }

    [Test]
    public void Query_WithMissingParameterColumn_ReportsInvalidArgument()
    {
        CreateDatabase();
        var parameters = new DataTable();
        parameters.Columns.Add("Name", typeof(string));
        var action = new QueryAction { DatabasePath = path, Sql = "SELECT 1", Parameters = parameters };

        Assert.That(Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!.Name,
            Is.EqualTo(ErrorCodes.InvalidArgument));
    }

    [Test]
    public void Query_WithMissingNameColumn_ReportsInvalidArgument()
    {
        CreateDatabase();
        var parameters = new DataTable();
        parameters.Columns.Add("Value", typeof(object));
        var action = new QueryAction { DatabasePath = path, Sql = "SELECT 1", Parameters = parameters };

        Assert.That(Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!.Name,
            Is.EqualTo(ErrorCodes.InvalidArgument));
    }

    [Test]
    public void Execute_WithUnnamedParameter_ReportsInvalidArgument()
    {
        CreateDatabase();
        var action = new ExecuteAction
        {
            DatabasePath = path,
            Sql = "SELECT 1",
            Parameters = Parameters(("", 1))
        };

        Assert.That(Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!.Name,
            Is.EqualTo(ErrorCodes.InvalidArgument));
    }

    [Test]
    public void Execute_WithEmptyParameterTable_Succeeds()
    {
        CreateDatabase();
        var action = new ExecuteAction
        {
            DatabasePath = path,
            Sql = "DELETE FROM records",
            Parameters = Parameters()
        };

        action.Execute(new ActionContext());

        Assert.That(action.AffectedRows, Is.Zero);
    }

    [Test]
    public void Resources_ContainModuleAndActionNames()
    {
        Assert.That(Properties.Resources.ResourceManager.GetString("SQLite_FriendlyName"), Is.EqualTo("SQLite"));
        Assert.That(Properties.Resources.ResourceManager.GetString("Query_FriendlyName"), Is.EqualTo("Query SQLite database"));
    }

    private void CreateDatabase()
    {
        new ExecuteAction
        {
            DatabasePath = path,
            CreateIfMissing = true,
            Sql = "CREATE TABLE records (value INTEGER)"
        }.Execute(new ActionContext());
    }

    private static DataTable Parameters(params (string Name, object Value)[] values)
    {
        var table = new DataTable();
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Value", typeof(object));
        foreach (var (name, value) in values)
        {
            table.Rows.Add(name, value);
        }

        return table;
    }
}
