using System;
using System.Data;
using System.IO;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.SQLite.Actions.Tests;

/// <summary>
///     Verifies SQLite query results without a server or ODBC driver.
/// </summary>
[TestFixture]
public sealed class QueryActionTests : SQLiteDatabaseFixture
{
    /// <summary>
    ///     Preserves database-null values in the returned DataTable.
    /// </summary>
    [Test]
    public void Execute_WhenValueIsNull_ReturnsDatabaseNull()
    {
        CreateDatabase();
        var parameters = CreateParameters("$value", DBNull.Value);
        var action = new QueryAction
        {
            DatabasePath = DatabasePath,
            Sql = "SELECT $value AS value",
            Parameters = parameters
        };

        RunAction(action);

        Assert.That(action.Result.Rows.Count, Is.EqualTo(1));
        Assert.That(action.Result.Rows[0].IsNull("value"), Is.True);
    }

    /// <summary>
    ///     Does not create a missing database during a read.
    /// </summary>
    [Test]
    public void Execute_WhenDatabaseIsMissing_ReportsDatabaseError()
    {
        var action = new QueryAction
        {
            DatabasePath = DatabasePath,
            Sql = "SELECT 1"
        };

        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.Database));
        Assert.That(File.Exists(DatabasePath), Is.False);
    }

    /// <summary>
    ///     Reports missing SQL before running a query.
    /// </summary>
    [Test]
    public void Execute_WhenSqlIsBlank_ReportsInvalidArgument()
    {
        CreateDatabase();
        var action = new QueryAction
        {
            DatabasePath = DatabasePath,
            Sql = " "
        };

        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.InvalidArgument));
    }

    /// <summary>
    ///     Rejects parameter rows missing the Value column.
    /// </summary>
    [Test]
    public void Execute_WhenValueColumnIsMissing_ReportsInvalidArgument()
    {
        CreateDatabase();
        var parameters = new DataTable();
        parameters.Columns.Add("Name", typeof(string));
        var action = new QueryAction
        {
            DatabasePath = DatabasePath,
            Sql = "SELECT 1",
            Parameters = parameters
        };

        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.InvalidArgument));
    }

    /// <summary>
    ///     Rejects parameter rows missing the Name column.
    /// </summary>
    [Test]
    public void Execute_WhenNameColumnIsMissing_ReportsInvalidArgument()
    {
        CreateDatabase();
        var parameters = new DataTable();
        parameters.Columns.Add("Value", typeof(object));
        var action = new QueryAction
        {
            DatabasePath = DatabasePath,
            Sql = "SELECT 1",
            Parameters = parameters
        };

        var error = Assert.Throws<ActionException>(() => RunAction(action))!;
        Assert.That(error.Name, Is.EqualTo(ErrorCodes.InvalidArgument));
    }
}
